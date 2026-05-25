using System;
using System.Collections;
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
using _Project.Code.Runtime.Gameplay.Temp;
using _Project.Code.Runtime.Gameplay.UI.Level;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Code.Runtime.Infrustructure.EntryPoints
{
    public class LevelEntryPoint : NetworkBehaviour
    {
        [SerializeField] private LevelUIMediator _levelUIMediator;
        [SerializeField] private Cage _cage;
        [SerializeField] private Cage _ratsCage;
        
        private IRolePicker _rolePicker;
        private ISceneLoader _sceneLoader;
        private IClientsRegistry _clientsRegistry;
        private ICharacterFactory _characterFactory;
        private IGameStateService _gameStateService;
        private IStaticDataService _staticDataService;
        private LoadingCurtain _loadingCurtain;
        private IWindowService _windowService;
        private ISlotService _slotService;
        
        private int _loadedClientsCount;
        private bool _isGameplayStarted = false;
        private CountdownTimer _countdownTimer;
        
        [Inject]
        private void Construct(IRolePicker rolePicker, ISceneLoader sceneLoader
            , IClientsRegistry clientsRegistry, ICharacterFactory characterFactory
            , IGameStateService gameStateService, IWindowService windowService,
            ISlotService slotService, ICameraFactory cameraFactory, IStaticDataService staticDataService
            , LoadingCurtain loadingCurtain)
        {
            _rolePicker = rolePicker;
            _sceneLoader = sceneLoader;
            _clientsRegistry = clientsRegistry;
            _characterFactory = characterFactory;
            _gameStateService = gameStateService;
            _windowService = windowService;
            _slotService = slotService;
            _staticDataService = staticDataService;
            _loadingCurtain = loadingCurtain;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnectedFromHost;
                return;
            }

            if (IsOwner)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            
            _slotService.PrepareLevelSlots();
            _gameStateService.OnAllHidersAreDead += OnAllHidersAreDead;
            _countdownTimer = new CountdownTimer();

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
                HideLoadingCurtainClientRpc();
                var hidingTime = _staticDataService.GameConfig.HidingTime;
                
                _countdownTimer.SetUp(hidingTime);
                _countdownTimer.OnElapsed += OnSeekerStartFinding;

                _levelUIMediator.BindTimer(_countdownTimer);
                _levelUIMediator.UpdateTimerIconBaseOnRoleClientRpc(GameRole.Hider);
                _levelUIMediator.ShowMassageClientRpc(GameRole.Hider, hidingTime);
                
                _countdownTimer.Start();
                
                _isGameplayStarted = true;
            }
        }
        
        private void OnSeekerStartFinding()
        {
            _cage.OpenClientRpc();
            
            var seeker = _gameStateService.GetSeekerGameplayState().Character;
            seeker.AllowJump(allow: true);
            seeker.AllowAttack(allow: true);
            
            _countdownTimer.Stop();
            _countdownTimer.OnElapsed -= OnSeekerStartFinding;
            
            var seekingTime = _staticDataService.GameConfig.SeekingTime;
            _countdownTimer.SetUp(seekingTime);
            _countdownTimer.OnElapsed += OnGameEndedDueToTimerStop;
            _countdownTimer.Start();
            _levelUIMediator.UpdateTimerIconBaseOnRoleClientRpc(GameRole.Seeker);
            _levelUIMediator.ShowMassageClientRpc(GameRole.Seeker);
        }

        private void OnGameEndedDueToTimerStop()
        {
            _countdownTimer.Stop();
            _countdownTimer.OnElapsed -= OnGameEndedDueToTimerStop;
            _levelUIMediator.UnBindTimer();

            RatWinsClientRpc();
            DeactivateAllClients();
            StartCoroutine(LoadLobby());
        }

        [ClientRpc]
        private void RatWinsClientRpc() => 
            _windowService.Open(WindowId.RatVictory);
        
        [ClientRpc]
        private void CatWinClientRpc() => 
            _windowService.Open(WindowId.CatVictory);
        
        [ClientRpc]
        private void HideLoadingCurtainClientRpc() => 
            _loadingCurtain.FadeIn();
        
        [ClientRpc]
        private void ProcessLoadingCurtainClientRpc() => 
            _loadingCurtain.Procced();

        [ServerRpc]
        private void LoadLobbyServerRpc() => 
            _sceneLoader.LoadSync("Lobby");
        
        private void DeactivateAllClients()
        {
            foreach (var state in _gameStateService.GameplayStates)
            {
                if (state == null) continue;
                
                if (state.Role == GameRole.Seeker)
                    state.Character.OnSeekerKilled -= PerformHiderDeath;
                
                state.Character.AllowJump(allow: false);
                state.Character.AllowMove(allow: false);
                state.Character.AllowLook(allow: false);
            }
        }
        
        private IEnumerator LoadLobby()
        {
            yield return new WaitForSeconds(2f);
            ProcessLoadingCurtainClientRpc();
            LoadLobbyServerRpc();
        }
        
        private void OnAllHidersAreDead()
        {
            CatWinClientRpc();
            DeactivateAllClients();
            StartCoroutine(LoadLobby());
        }
        
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
            {
                character.AllowJump(allow: false);
                character.AllowAttack(allow: false);
                character.OnSeekerKilled += PerformHiderDeath;
            }
            
            _gameStateService.AddClientGameplayState(profile, character);
        }

        private void PerformHiderDeath(ulong hiderId)
        {
            if (_gameStateService.IsClientByIdDead(hiderId)) return;
            
            ClientGameplayState hider = _gameStateService.SetClientGameplayStateToDead(hiderId);
            
            hider.Character.Teleport(_ratsCage.CenterPoint.position);
            hider.Character.AllowJump(allow: false);
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
            _gameStateService.OnAllHidersAreDead -= OnAllHidersAreDead;
        }
    }
}