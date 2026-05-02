using System;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.StaticData;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Code.Runtime.Gameplay.UI.Lobby
{
    public class SwapRoleWindow : MonoBehaviour
    {
        [Header("Base")]
        [SerializeField] private RectTransform _root;
        [SerializeField] private TextMeshProUGUI _requesterName;
        
        [Header("Buttons")]
        [SerializeField] private Button _acceptSwapRoleButton;
        [SerializeField] private Button _declineSwapRoleButton;
        
        [Header("PreviewImages")]
        [SerializeField] private Image _fromRoleImage;
        [SerializeField] private Image _toRoleImage;
        
        private IStaticDataService _staticDataService;
        
        [Inject]
        private void Construct(IStaticDataService staticDataService) => 
            _staticDataService = staticDataService;

        public void Show(FixedString64Bytes requesterName, GameRole requesterRole,
            Action onAccepted, Action onDeclined)
        {
            _root.gameObject.SetActive(true);

            _requesterName.text = requesterName.ToString();
            
            _fromRoleImage.sprite = requesterRole == GameRole.Hider
                ? _staticDataService.GameConfig.RatSprite
                : _staticDataService.GameConfig.CatSprite;
            
            _toRoleImage.sprite = requesterRole == GameRole.Hider
                ? _staticDataService.GameConfig.CatSprite
                : _staticDataService.GameConfig.RatSprite;
            
            Clear();
            
            _acceptSwapRoleButton.onClick.AddListener(() => onAccepted?.Invoke());
            _declineSwapRoleButton.onClick.AddListener(() => onDeclined?.Invoke());
        }

        public void Hide()
        {
            _root.gameObject.SetActive(false);
            Clear();
        }

        private void Clear()
        {
            _acceptSwapRoleButton.onClick.RemoveAllListeners();
            _declineSwapRoleButton.onClick.RemoveAllListeners();
        }
    }
}