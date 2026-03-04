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
        
        [Inject]
        private void Construct(IRolePicker rolePicker, ISceneLoader sceneLoader)
        {
            _rolePicker = rolePicker;
            _sceneLoader = sceneLoader;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                _hub.gameObject.SetActive(false);
                return;
            }
            
            _exitButton.onClick.AddListener(() => {
                _sceneLoader.LoadSync("Lobby");
            });
        }

        public override void OnNetworkDespawn() => 
            _exitButton.onClick.RemoveAllListeners();
    }
}