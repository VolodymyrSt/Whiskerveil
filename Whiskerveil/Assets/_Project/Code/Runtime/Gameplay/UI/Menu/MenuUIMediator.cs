using _Project.Code.Runtime.CommonServices.Network;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Code.Runtime.Gameplay.UI.Menu
{
    public class MenuUIMediator : MonoBehaviour
    {
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _clientButton;
        [SerializeField] private TMP_InputField _nicknameInputField;
        [SerializeField] private TMP_InputField _roomCodeInputField;
        
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
            _hostButton.onClick.AddListener(() =>
                _hostNetworkService.StartHost(_nicknameInputField.text).Forget());
            
            _clientButton.onClick.AddListener(() => 
                _clientNetworkService.StartClient(_nicknameInputField.text, _roomCodeInputField.text).Forget());
        }

        private void OnDestroy()
        {
            _hostButton.onClick.RemoveAllListeners();
            _clientButton.onClick.RemoveAllListeners();
        }
    }
}