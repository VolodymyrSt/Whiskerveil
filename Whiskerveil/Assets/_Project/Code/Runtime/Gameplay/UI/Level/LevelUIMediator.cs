using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.SceneLoader;
using _Project.Code.Runtime.CommonServices.TimeManagement;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Code.Runtime.Gameplay.UI.Level
{
    public class LevelUIMediator : NetworkBehaviour
    {
        [SerializeField] private Button _exitButton;
        
        [Header("Timer")]
        [SerializeField] private CountdownTimerView _countdownTimerView;
        
        [Header("Massage")]
        [SerializeField] private RectTransform _massageRoot;
        [SerializeField] private TextMeshProUGUI _massageText;
        
        private ISceneLoader _sceneLoader;
        private CountdownTimer _timer;
        
        [Inject]
        private void Construct(ISceneLoader sceneLoader) => 
            _sceneLoader = sceneLoader;

        public override void OnNetworkSpawn()
        {
            _massageRoot.localScale = Vector3.zero;
            _massageRoot.gameObject.SetActive(false);
            
            if (!IsServer)
            {
                _exitButton.gameObject.SetActive(false);
                return;
            }
            
            _exitButton.onClick.AddListener(() => {
                _sceneLoader.LoadSync("Lobby");
            });
        }

        public void BindTimer(CountdownTimer timer)
        {
            _timer = timer;
            _timer.OnSecondElapsed += OnTimerSecondTicked;
        }
        
        public void UnBindTimer()
        {
            if (_timer != null)
            {
                _timer.OnSecondElapsed -= OnTimerSecondTicked;
                _timer = null;
            }
        }

        private void OnTimerSecondTicked(int seconds) => 
            UpdateTimerClientRpc(seconds);

        [ClientRpc]
        public void ShowMassageClientRpc(GameRole role)
        {
            _massageRoot.gameObject.SetActive(true);
            var massage = role ==  GameRole.Hider ? "YOU HAVE 30 SECONDS TO HIDE" : "THE CAR IS FREE";
            _massageText.text = massage;
            _massageRoot.DOScale(1f, 0.5f)
                .SetEase(Ease.Linear)
                .OnComplete(() => {
                    HideMassageDelayed(5f);
                })
                .Play();
        }
        
        [ClientRpc]
        public void UpdateTimerIconBaseOnRoleClientRpc(GameRole role) => 
            _countdownTimerView.SetIconBaseOnRole(role);
        
        [ClientRpc]
        private void UpdateTimerClientRpc(int seconds) => 
            _countdownTimerView.UpdateTimerText(remaining: seconds);

        private void HideMassageDelayed(float delay)
        {
            _massageRoot.DOScale(0, 0.5f)
                .SetDelay(delay)
                .SetEase(Ease.Linear)
                .OnComplete(() => {
                    _massageRoot.gameObject.SetActive(false);
                })
                .Play();
        }
        
        public override void OnNetworkDespawn() => 
            _exitButton.onClick.RemoveAllListeners();
    }
}