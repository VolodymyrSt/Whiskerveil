using _Project.Code.Runtime.CommonServices.SceneLoader;
using _Project.Code.Runtime.CommonServices.WindowManagement;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Code.Runtime.UI.Windows
{
    public class LeaveLobbyWindow : BaseWindow
    {
        [Header("Button")]
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Button _cancelButton;

        private ISceneLoader _sceneLoader;
        private IWindowService _windowService;
        
        [Inject]
        private void Construct(ISceneLoader sceneLoader, IWindowService windowService)
        {
            _sceneLoader = sceneLoader;
            _windowService = windowService;
        }

        protected override void Initialize()
        {
            Id = WindowId.LeaveLobby;
            _leaveButton.onClick.AddListener(() => {
                NetworkManager.Singleton.Shutdown();
                _sceneLoader.Load(SceneList.Menu);
                _leaveButton.onClick.RemoveAllListeners();
            });

            _cancelButton.onClick.AddListener(() => {
                _windowService.Close(Id);
                _cancelButton.onClick.RemoveAllListeners();
            });
        }
    }
}