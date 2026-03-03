using _Project.Code.Runtime.CommonServices.NetworkServices;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Code.Runtime.UI.Menu
{
    public class MenuUIMediator : MonoBehaviour
    {
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _clientButton;
        
        private IHostNetworkService _hostNetworkService;
        private IClientNetworkService _clientNetworkService;

        [Inject]
        private void Construct(IHostNetworkService hostNetworkService, IClientNetworkService clientNetworkService)
        {
            _hostNetworkService = hostNetworkService;
            _clientNetworkService = clientNetworkService;
        }
        
        private void Awake()
        {
            _hostButton.onClick.AddListener((() =>
            {
                _hostNetworkService.StartHost();
            }));
            
            _clientButton.onClick.AddListener((() =>
            {
                _clientNetworkService.StartClient();
            }));
        }

        private void OnDestroy()
        {
            _hostButton.onClick.RemoveAllListeners();
            _clientButton.onClick.RemoveAllListeners();
        }
    }
}