using System.Collections.Generic;
using _Project.Code.Runtime.Character.Factory;
using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.GameState;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.SceneLoader;
using _Project.Code.Runtime.CommonServices.SlotsManagement;
using _Project.Code.Runtime.CommonServices.StaticData;
using _Project.Code.Runtime.CommonServices.WindowManagement;
using _Project.Code.Runtime.Configs.Slots;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Code.Runtime.Infrustructure.EntryPoints
{
    public class LevelEntryPoint : NetworkBehaviour
    {
        private IRolePicker _rolePicker;
        private ISceneLoader _sceneLoader;
        private IClientsRegistry _clientsRegistry;
        private ICharacterFactory _characterFactory;
        private IGameStateService _gameStateService;
        private IWindowService _windowService;
        private ISlotService _slotService;
        
        [Inject]
        private void Construct(IRolePicker rolePicker, ISceneLoader sceneLoader
            , IClientsRegistry clientsRegistry, ICharacterFactory characterFactory
            , IGameStateService gameStateService, IWindowService windowService,
            ISlotService slotService)
        {
            _rolePicker = rolePicker;
            _sceneLoader = sceneLoader;
            _clientsRegistry = clientsRegistry;
            _characterFactory = characterFactory;
            _gameStateService = gameStateService;
            _windowService = windowService;
            _slotService = slotService;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnectedFromHost;
                return;
            }
            
            _slotService.PrepareLevelSlots();

            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnLevelLoaded;
        }
        
        private void OnLevelLoaded(SceneEvent sceneEvent)
        {
            if (sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;
            LoadClientById(sceneEvent.ClientId);
        }
        
        private void LoadClientById(ulong clientId)
        {
            ClientProfile profile = _clientsRegistry.Profiles.Find(x => x.Id == clientId);

            if (profile != null)
                ConfigureClientByProfile(profile);
        }
        
        private void ConfigureClientByProfile(ClientProfile profile)
        {
            var availableSlot = _slotService.GetFreeLevelSlotFor(profile.Role);
            availableSlot.IsTaken = true;
            _characterFactory.CreateCharacterByProfile(profile, availableSlot.Position, availableSlot.Rotation);
        }

        private void OnDisconnectedFromHost(ulong clientId)
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
                _windowService.Open(WindowId.HostDisconnect);
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer)
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnDisconnectedFromHost;
            
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnLevelLoaded;
        }
    }
}