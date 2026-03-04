using _Project.Code.Runtime.Character;
using _Project.Code.Runtime.Character.Factory;
using _Project.Code.Runtime.CommonServices.HostLobbyState;
using _Project.Code.Runtime.CommonServices.LobbySlots;
using _Project.Code.Runtime.CommonServices.Network;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.SceneLoader;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Code.Runtime.Infrustructure.EntryPoints
{
    public class LobbyEntryPoint : NetworkBehaviour
    {
        [SerializeField] private Canvas _hub;
        [SerializeField] private Button _readyButton;
        
        private IHostNetworkService _hostNetworkService;
        private ILobbySlotService _lobbySlotService;
        private ISceneLoader _sceneLoader;
        private ICharacterFactory _characterFactory;
        private IHostLobbyStateService _hostLobbyStateService;

        [Inject]
        public void Construct(IHostNetworkService hostNetworkService, ILobbySlotService lobbySlotService
            , ICharacterFactory characterFactory, IHostLobbyStateService hostLobbyStateService
            , ISceneLoader sceneLoader)
        {
            _hostNetworkService = hostNetworkService;
            _lobbySlotService = lobbySlotService;
            _characterFactory = characterFactory;
            _hostLobbyStateService = hostLobbyStateService;
            _sceneLoader = sceneLoader;
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                _hub.gameObject.SetActive(false);
                return;
            }
            
            _readyButton.onClick.AddListener(() => {
                _sceneLoader.LoadSync("Level");
            });
            
            _hostNetworkService.OnHostJoinedLobby += OnHostJoinedLobby;
            _hostNetworkService.OnClientJoinedLobby += OnClientJoinedLobby;
        }

        private void OnHostJoinedLobby(ulong hostId)
        {
            ICharacter hostPlayerPrefab = CreateHostPlayerPrefab(hostId);
            _hostLobbyStateService.BindPlayerToSlot(hostId, hostPlayerPrefab);
        }

        private void OnClientJoinedLobby(ulong clientId)
        {
            ICharacter clientPlayerPrefab = CreateClientPlayerPrefab(clientId);
            _hostLobbyStateService.BindPlayerToSlot(clientId, clientPlayerPrefab);
        }
        
        private ICharacter CreateClientPlayerPrefab(ulong clientId) => 
            _characterFactory.CreateCharacter(clientId, GameRole.Hider);

        private ICharacter CreateHostPlayerPrefab(ulong hostId) => 
            _characterFactory.CreateCharacter(hostId, GameRole.Seeker);

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            
            _hostNetworkService.OnClientJoinedLobby -= OnClientJoinedLobby;
            _hostNetworkService.OnHostJoinedLobby -= OnHostJoinedLobby;
            _readyButton.onClick.RemoveAllListeners();
        }
    }
}