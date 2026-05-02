using System;
using System.Collections.Generic;
using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.GameState;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.SceneLoader;
using _Project.Code.Runtime.CommonServices.SlotsManagement;
using _Project.Code.Runtime.CommonServices.StaticData;
using _Project.Code.Runtime.CommonServices.TimeManagement;
using _Project.Code.Runtime.CommonServices.WindowManagement;
using _Project.Code.Runtime.Configs.Slots;
using _Project.Code.Runtime.Gameplay.Camera.Factory;
using _Project.Code.Runtime.Gameplay.Character;
using _Project.Code.Runtime.Gameplay.Character.Factory;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Code.Runtime.Infrustructure.EntryPoints
{
    public class LevelEntryPoint : NetworkBehaviour
    {
        private readonly NetworkVariable<float> _remainingTime = new(writePerm: NetworkVariableWritePermission.Server);

        [SerializeField] private CountdownTimerView _countdownTimerView;
        
        private IRolePicker _rolePicker;
        private ISceneLoader _sceneLoader;
        private IClientsRegistry _clientsRegistry;
        private ICharacterFactory _characterFactory;
        private IGameStateService _gameStateService;
        private IStaticDataService _staticDataService;
        private IWindowService _windowService;
        private ISlotService _slotService;
        
        private int _loadedClientsCount;
        private bool _isGameplayStarted = false;
        private CountdownTimer _countdownTimer;
        
        [Inject]
        private void Construct(IRolePicker rolePicker, ISceneLoader sceneLoader
            , IClientsRegistry clientsRegistry, ICharacterFactory characterFactory
            , IGameStateService gameStateService, IWindowService windowService,
            ISlotService slotService, ICameraFactory cameraFactory, IStaticDataService staticDataService)
        {
            _rolePicker = rolePicker;
            _sceneLoader = sceneLoader;
            _clientsRegistry = clientsRegistry;
            _characterFactory = characterFactory;
            _gameStateService = gameStateService;
            _windowService = windowService;
            _slotService = slotService;
            _staticDataService = staticDataService;
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


        private void Update()
        {
            if (!IsServer || !_isGameplayStarted) return;
            
            _countdownTimer.Tick();
        }

        private void OnLevelLoaded(SceneEvent sceneEvent)
        {
            if (sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;
            LoadClientById(sceneEvent.ClientId);
            _loadedClientsCount++;
    
            if (IsAllClientLoaded())
            {
                var hidingTime = _staticDataService.GameConfig.HidingTime;
                _countdownTimer = new CountdownTimer();
                _countdownTimer.SetUp(hidingTime);
                _countdownTimer.OnElapsed += OnSeekerStartFinding;
                _countdownTimer.OnSecondElapsed += OnTimerSecondTicked;
                _countdownTimer.Start();
                
                BeginGameplayClientRpc();
                _isGameplayStarted = true;
            }
        }
        
        private void OnSeekerStartFinding()
        {
            _countdownTimer.OnElapsed -= OnSeekerStartFinding;
            _countdownTimer.OnSecondElapsed -= OnTimerSecondTicked;
            
            var seekingTime = _staticDataService.GameConfig.SeekingTime;
            _countdownTimer.SetUp(seekingTime);
            _countdownTimer.OnElapsed += OnHidersLive;
            _countdownTimer.OnSecondElapsed += OnTimerSecondTicked;
            _countdownTimer.Start();
        }

        private void OnHidersLive()
        {
            Debug.Log("Win");
        }

        private void OnTimerSecondTicked(int seconds) => 
            UpdateTimerClientRpc(seconds);

        [ClientRpc]
        private void BeginGameplayClientRpc(ClientRpcParams rpcParams = default)
        {

        }
        
        [ClientRpc]
        private void UpdateTimerClientRpc(int seconds) => 
            _countdownTimerView.UpdateTimerText(remaining: seconds);

        private bool IsAllClientLoaded() => 
            _loadedClientsCount >= _clientsRegistry.Profiles.Count;
        
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
            ICharacter character = _characterFactory.CreateCharacterByProfile(profile, availableSlot.Position, availableSlot.Rotation);
            
            if (profile.Role == GameRole.Seeker)
                character.AllowJump(allow: false);
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