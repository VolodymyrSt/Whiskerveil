using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.GameState;
using _Project.Code.Runtime.CommonServices.InputManagement;
using _Project.Code.Runtime.CommonServices.Network;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.SceneLoader;
using _Project.Code.Runtime.CommonServices.SlotsManagement;
using _Project.Code.Runtime.CommonServices.WindowManagement;
using _Project.Code.Runtime.Gameplay.Character.Factory;
using _Project.Code.Runtime.Gameplay.Character.Preview;
using _Project.Code.Runtime.Gameplay.Character.Preview.Factory;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Infrustructure.EntryPoints
{
    public class LobbyEntryPoint : NetworkBehaviour
    {
        private IHostNetworkService _hostNetworkService;
        private ISlotService _slotService;
        private ICharacterPreviewFactory _characterPreviewFactory;
        private IGameStateService _gameStateService;
        private IClientsRegistry _clientsRegistry;
        private IWindowService _windowService;
        private ISceneLoader _sceneLoader;
        private IRolePicker _rolePicker;
        private IInputService _input;

        [Inject]
        public void Construct(IHostNetworkService hostNetworkService, ISlotService slotService
            , ICharacterPreviewFactory characterFactory, IClientsRegistry clientsRegistry, IGameStateService gameStateService
            ,ISceneLoader sceneLoader, IWindowService windowService, IRolePicker rolePicker, IInputService inputService)
        {
            _hostNetworkService = hostNetworkService;
            _slotService = slotService;
            _characterPreviewFactory = characterFactory;
            _clientsRegistry = clientsRegistry;
            _gameStateService = gameStateService;
            _windowService = windowService;
            _sceneLoader = sceneLoader;
            _rolePicker = rolePicker;
            _input = inputService;
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnectedFromHost;
                return;
            }
            
            if (IsOwner)
                _input.Enable();
            
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
                randomHiderLobbyState.Preview.AssignRole(GameRole.Seeker);
                randomHiderLobbyState.Preview.Transform.SetPositionAndRotation(
                    disconnectedClientSlot.Position, disconnectedClientSlot.Rotation);
                
                randomHiderClientSlot.IsTaken = false;
                disconnectedClientSlot.IsTaken = true;
                randomHiderClientProfile.SlotId = disconnectedClientSlot.Id;
                
                _rolePicker.RestoreRole(GameRole.Hider);
            }
            else
            {
                disconnectedClientSlot.IsTaken = false;
                _rolePicker.RestoreRole(GameRole.Hider);
            }

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
            
            IPreview preview = _characterPreviewFactory.CreatePreviewByProfile(profile, slot.Position, slot.Rotation);
            _gameStateService.AddClientLobbyState(profile, preview);
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