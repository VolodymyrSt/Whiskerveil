using _Project.Code.Runtime.Character;
using _Project.Code.Runtime.Character.Factory;
using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.GameState;
using _Project.Code.Runtime.CommonServices.LobbySlots;
using _Project.Code.Runtime.CommonServices.Network;
using _Project.Code.Runtime.CommonServices.SceneLoader;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Infrustructure.EntryPoints
{
    public class LobbyEntryPoint : NetworkBehaviour
    {
        private IHostNetworkService _hostNetworkService;
        private ILobbySlotService _lobbySlotService;
        private ICharacterFactory _characterFactory;
        private IGameStateService _gameStateService;
        private IClientsRegistry _clientsRegistry;
        private ISceneLoader _sceneLoader;

        [Inject]
        public void Construct(IHostNetworkService hostNetworkService, ILobbySlotService lobbySlotService
            , ICharacterFactory characterFactory, IClientsRegistry clientsRegistry, IGameStateService gameStateService
            ,ISceneLoader sceneLoader)
        {
            _hostNetworkService = hostNetworkService;
            _lobbySlotService = lobbySlotService;
            _characterFactory = characterFactory;
            _clientsRegistry = clientsRegistry;
            _gameStateService = gameStateService;
            _sceneLoader = sceneLoader;
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            
            _gameStateService.ClearClientLobbyStates();
            _gameStateService.OnAllClientReadyToPlay += OnAllClientsReady;
            
            _clientsRegistry.OnNewClientAdded += ConfigureClientByProfile;
            _hostNetworkService.OnClientDisconnected += OnClientDisconnected;
            
            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnLobbyLoaded;
        }

        private void OnClientDisconnected(ulong clientId)
        {
            NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.Despawn();
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
                slot = _lobbySlotService.GetFreeSlotFor(profile.Role);
                profile.SlotId = slot.Id;
                slot.IsTaken = true;
            }
            else
                slot = _lobbySlotService.GetSlotById(profile.SlotId);
            
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

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            
            _gameStateService.OnAllClientReadyToPlay -= OnAllClientsReady;
            _clientsRegistry.OnNewClientAdded -= ConfigureClientByProfile;
            _hostNetworkService.OnClientDisconnected -= OnClientDisconnected;
            
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnLobbyLoaded;
        }
    }
}