using _Project.Code.Runtime.CommonServices.SceneLoader;
using _Project.Code.Runtime.CommonServices.WindowManagement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Code.Runtime.UI.Windows
{
    public class HostDisconnectWindow : BaseWindow
    {
        [Header("Button")]
        [SerializeField] private Button _goMenuButton;

        private ISceneLoader _sceneLoader;
        
        [Inject]
        private void Construct(ISceneLoader sceneLoader) => 
            _sceneLoader = sceneLoader;
        
        protected override void Initialize()
        {
            Id = WindowId.HostDisconnect;
            _goMenuButton.onClick.AddListener(() => {
                _sceneLoader.Load(SceneList.Menu);
                _goMenuButton.onClick.RemoveAllListeners();
            });
        }
    }
}