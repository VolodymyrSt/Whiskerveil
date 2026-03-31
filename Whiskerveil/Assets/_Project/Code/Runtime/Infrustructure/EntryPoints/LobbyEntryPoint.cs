using _Project.Code.Runtime.Character;
using _Project.Code.Runtime.Character.Factory;
using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.GameState;
using _Project.Code.Runtime.CommonServices.Network;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.SceneLoader;
using _Project.Code.Runtime.CommonServices.SlotsManagement;
using _Project.Code.Runtime.CommonServices.WindowManagement;
using _Project.Code.Runtime.UI.Lobby;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Infrustructure.EntryPoints
{
    public class LobbyEntryPoint : NetworkBehaviour
    {
        private IHostNetworkService _hostNetworkService;
        private ISlotService _slotService;
        private ICharacterFactory _characterFactory;
        private IGameStateService _gameStateService;
        private IClientsRegistry _clientsRegistry;
        private IWindowService _windowService;
        private ISceneLoader _sceneLoader;
        private IRolePicker _rolePicker;

        [Inject]
        public void Construct(IHostNetworkService hostNetworkService, ISlotService slotService
            , ICharacterFactory characterFactory, IClientsRegistry clientsRegistry, IGameStateService gameStateService
            ,ISceneLoader sceneLoader, IWindowService windowService, IRolePicker rolePicker)
        {
            _hostNetworkService = hostNetworkService;
            _slotService = slotService;
            _characterFactory = characterFactory;
            _clientsRegistry = clientsRegistry;
            _gameStateService = gameStateService;
            _windowService = windowService;
            _sceneLoader = sceneLoader;
            _rolePicker = rolePicker;
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnectedFromHost;
                return;
            }
            
            _slotService.PrepareLobbySlots();
            
            _gameStateService.PrepairForClientConnection();
            _gameStateService.OnAllClientReadyToPlay += OnAllClientsReady;
            
            _clientsRegistry.OnNewClientAdded += ConfigureClientByProfile;
            _hostNetworkService.OnClientDisconnected += OnClientDisconnected;
            
            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnLobbyLoaded;
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (clientId == 0) return;

            ClientProfile disconnectedClientProfile = _clientsRegistry.GetById(clientId);
            LobbySlot disconnectedClientSlot = _slotService.GetLobbySlotById(disconnectedClientProfile.SlotId);

            if (disconnectedClientProfile.Role == GameRole.Seeker)
            {
                ClientProfile randomHiderClientProfile = _clientsRegistry.GetFirstByRole(GameRole.Hider);
                ClientLobbyState randomHiderLobbyState = _gameStateService.GetClientLobbyStateById(randomHiderClientProfile.Id);
                LobbySlot randomHiderClientSlot = _slotService.GetLobbySlotById(randomHiderClientProfile.SlotId);
                
                randomHiderClientProfile.Role = GameRole.Seeker;
                randomHiderLobbyState.Character.AssignRole(GameRole.Seeker);
                randomHiderLobbyState.Character.Transform.SetPositionAndRotation(
                    disconnectedClientSlot.Position, disconnectedClientSlot.Rotation);
                
                randomHiderClientSlot.IsTaken = false;
                disconnectedClientSlot.IsTaken = true;
                randomHiderClientProfile.SlotId = disconnectedClientSlot.Id;
            }
            else
                disconnectedClientSlot.IsTaken = false;
            
            _rolePicker.RestoreRole(GameRole.Hider);
            _clientsRegistry.RemoveProfile(clientId);
            _gameStateService.RemoveClientLobbyState(clientId);
        }
        
        private void OnLobbyLoaded(SceneEvent sceneEvent)
        {
            if (sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;
            LoadClientById(sceneEvent.ClientId);
        }

        private void ConfigureClientByProfile(ClientProfile profile)
        {
            LobbySlot slot;
            
            if (profile.SlotId.IsEmpty)
            {
                slot = _slotService.GetFreeLobbySlotFor(profile.Role);
                profile.SlotId = slot.Id;
                slot.IsTaken = true;
            }
            else
                slot = _slotService.GetLobbySlotById(profile.SlotId);
            
            ICharacter character = _characterFactory.CreateCharacterByProfile(profile, slot.Position, slot.Rotation);
            _gameStateService.AddClientLobbyState(profile, character);
        }
        
        private void LoadClientById(ulong clientId)
        {
            ClientProfile profile = _clientsRegistry.Profiles.Find(x => x.Id == clientId);

            if (profile != null)
                ConfigureClientByProfile(profile);
        }
        
        private void OnAllClientsReady()
        {
            _sceneLoader.LoadSync(SceneList.Level, () => 
                _gameStateService.SetSceneState(SceneState.InLevel));
        }
        
        private void OnDisconnectedFromHost(ulong clientId)
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
                _windowService.Open(WindowId.HostDisconnect);
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnDisconnectedFromHost;
                return;
            }
            
            _gameStateService.OnAllClientReadyToPlay -= OnAllClientsReady;
            _clientsRegistry.OnNewClientAdded -= ConfigureClientByProfile;
            _hostNetworkService.OnClientDisconnected -= OnClientDisconnected;
            
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnLobbyLoaded;
        }
    }
}