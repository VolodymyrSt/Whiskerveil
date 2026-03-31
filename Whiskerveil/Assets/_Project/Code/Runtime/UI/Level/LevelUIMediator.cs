using _Project.Code.Runtime.CommonServices.SceneLoader;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Code.Runtime.UI.Level
{
    public class LevelUIMediator : NetworkBehaviour
    {
        [SerializeField] private Button _exitButton;
        
        private ISceneLoader _sceneLoader;
        
        [Inject]
        private void Construct(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                _exitButton.gameObject.SetActive(false);
                return;
            }
            
            _exitButton.onClick.AddListener(() => {
                _sceneLoader.LoadSync("Lobby");
            });
        }
        
        public override void OnNetworkDespawn()
        {
            _exitButton.onClick.RemoveAllListeners();
        }
    }
}