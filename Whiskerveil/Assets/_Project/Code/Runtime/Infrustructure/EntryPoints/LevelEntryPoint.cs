using _Project.Code.Runtime.Character.Factory;
using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.GameState;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.SceneLoader;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Code.Runtime.Infrustructure.EntryPoints
{
    public class LevelEntryPoint : NetworkBehaviour
    {
        [SerializeField] private Canvas _hub;
        [SerializeField] private Button _exitButton;
        
        private IRolePicker _rolePicker;
        private ISceneLoader _sceneLoader;
        private IClientsRegistry _clientsRegistry;
        private ICharacterFactory _characterFactory;
        private IGameStateService _gameStateService;
        
        [Inject]
        private void Construct(IRolePicker rolePicker, ISceneLoader sceneLoader
            , IClientsRegistry clientsRegistry, ICharacterFactory characterFactory
            , IGameStateService gameStateService)
        {
            _rolePicker = rolePicker;
            _sceneLoader = sceneLoader;
            _clientsRegistry = clientsRegistry;
            _characterFactory = characterFactory;
            _gameStateService = gameStateService;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                _hub.gameObject.SetActive(false);
                return;
            }

            foreach (var profile in _clientsRegistry.Profiles)
                _characterFactory.CreateCharacterByProfile(profile, Vector3.back, Quaternion.identity);
            
            _exitButton.onClick.AddListener(() => {
                _sceneLoader.LoadSync("Lobby");
            });
        }

        public override void OnNetworkDespawn() => 
            _exitButton.onClick.RemoveAllListeners();
    }
}