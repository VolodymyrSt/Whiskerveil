using System.Collections.Generic;
using _Project.Code.Runtime.CommonServices.RolePicker;
using TMPro;
using UnityEngine;

namespace _Project.Code.Runtime.Gameplay.Character.View
{
    public class CharacterView : MonoBehaviour, ICharacterView
    {
        private readonly int WalkingHash = Animator.StringToHash("IsWalking");
        private readonly int SprintingHash = Animator.StringToHash("IsSprinting");
        private readonly int JumpHash = Animator.StringToHash("Jump");
        
        [SerializeField] private List<ViewByRoleData> _viewByRoles = new List<ViewByRoleData>();
        
        [Header("Nickname")]
        [SerializeField] private RectTransform _nicknameRoot;
        [SerializeField] private TextMeshProUGUI _nickname;
        
        private bool _isRoleInitialized;
        private bool _wasJumping;
        private ViewByRoleData _currentRoleData;

        public void UpdateName(string characterName) => 
            _nickname.text = characterName;
        
        public void SetVisible(bool visible) => 
            gameObject.SetActive(visible);

        public void UpdateAnimation(bool isWalking, bool sprinting, bool jump)
        {
            if (!_isRoleInitialized) return;
            
            _currentRoleData.Animator.SetBool(WalkingHash, isWalking && !sprinting);
            _currentRoleData.Animator.SetBool(SprintingHash, sprinting && isWalking);

            if (!_wasJumping && jump)
                _currentRoleData.Animator.SetTrigger(JumpHash);
            
            _wasJumping = jump;
        }
        
        public void HideNick() =>
            _nickname.gameObject.SetActive(false);
        
        public virtual void SwitchViewBaseOnRole(GameRole role)
        {
            foreach (var data in _viewByRoles)
            {
                if (data.Role == role)
                {
                    _currentRoleData = data;
                    data.View.gameObject.SetActive(true);
                }
                else
                    data.View.gameObject.SetActive(false);
            }
            
            UpdateNicknamePositionBaseOnView(role);
            _isRoleInitialized = true;
        }
        
        public void UpdateNicknamePositionBaseOnView(GameRole role)
        {
            _nicknameRoot.anchoredPosition = role == GameRole.Hider ?
                new(_nicknameRoot.anchoredPosition.x, 0.42f) :
                new(_nicknameRoot.anchoredPosition.x, 0.72f); //constants
        }
    }
}
