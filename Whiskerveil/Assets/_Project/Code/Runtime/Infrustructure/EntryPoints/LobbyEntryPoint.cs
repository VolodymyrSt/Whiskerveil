using System.Collections.Generic;
using _Project.Code.Runtime.Character;
using _Project.Code.Runtime.Character.Factory;
using _Project.Code.Runtime.CommonServices.LobbySlots;
using _Project.Code.Runtime.CommonServices.Network;
using _Project.Code.Runtime.CommonServices.PlayerRegistry;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.SceneLoader;
using ModestTree;
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
        
        private readonly List<ICharacter> _clientPrefabs = new List<ICharacter>();
        
        private IHostNetworkService _hostNetworkService;
        private ILobbySlotService _lobbySlotService;
        private ISceneLoader _sceneLoader;
        private ICharacterFactory _characterFactory;
        private IPlayersRegistry _playersRegistry;

        [Inject]
        public void Construct(IHostNetworkService hostNetworkService, ILobbySlotService lobbySlotService
            , ICharacterFactory characterFactory, ISceneLoader sceneLoader, IPlayersRegistry playersRegistry)
        {
            _hostNetworkService = hostNetworkService;
            _lobbySlotService = lobbySlotService;
            _characterFactory = characterFactory;
            _sceneLoader = sceneLoader;
            _playersRegistry = playersRegistry;
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

            if (!_playersRegistry.IsEmpty)
            {
                foreach (PlayerProfile profile in _playersRegistry.Profiles)
                    ConfigurePlayerByProfile(profile);
            }
            
            _playersRegistry.OnNewPlayerAdded += ConfigurePlayerByProfile;
            _hostNetworkService.OnClientDisconnected += OnClientDisconnected;
        }

        private void OnClientDisconnected(ulong clientId)
        {
            var clientPrefab = _clientPrefabs.Find(x => x.Id == clientId);
            NetworkObject.Destroy(clientPrefab.Transform.gameObject);
        }

        private void ConfigurePlayerByProfile(PlayerProfile profile)
        {
            LobbySlot slot;
            
            if (string.IsNullOrEmpty(profile.SlotId))
            {
                slot = _lobbySlotService.GetFreeSlotFor(profile.Role);
                slot.IsTaken = true;
                profile.SlotId = slot.Id;
            }
            else
                slot = _lobbySlotService.GetSlotById(profile.SlotId);
            
            ICharacter player = _characterFactory.CreateCharacter(profile.Id, profile.Role, slot.Position, slot.Rotation);
            _clientPrefabs.Add(player);
        }

        private void SwapRoles()
        {
            
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            
            _playersRegistry.OnNewPlayerAdded -= ConfigurePlayerByProfile;
            _hostNetworkService.OnClientDisconnected -= OnClientDisconnected;
            _readyButton.onClick.RemoveAllListeners();
        }
    }
}