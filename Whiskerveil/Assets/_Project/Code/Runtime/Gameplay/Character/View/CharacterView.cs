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
        
        [SerializeField] private List<ViewByRoleData> _viewByRoles = new List<ViewByRoleData>();
        
        [Header("Nickname")]
        [SerializeField] private RectTransform _nicknameRoot;
        [SerializeField] private TextMeshProUGUI _nickname;

        public void UpdateName(string characterName) => 
            _nickname.text = characterName;
        
        public void HideNick() =>
            _nickname.gameObject.SetActive(false);
        
        public virtual void SwitchViewBaseOnRole(GameRole role)
        {
            foreach (var data in _viewByRoles)
            {
                if (data.Role == role)
                    data.View.gameObject.SetActive(true);
                else
                    data.View.gameObject.SetActive(false);
            }
            
            UpdateNicknamePositionBaseOnView(role);
        }
        
        public void UpdateNicknamePositionBaseOnView(GameRole role)
        {
            _nicknameRoot.anchoredPosition = role == GameRole.Hider ?
                new(_nicknameRoot.anchoredPosition.x, 0.42f) :
                new(_nicknameRoot.anchoredPosition.x, 0.72f); //constants
        }
    }
}
