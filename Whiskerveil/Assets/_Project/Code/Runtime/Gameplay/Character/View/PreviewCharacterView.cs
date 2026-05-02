using _Project.Code.Runtime.CommonServices.RolePicker;
using UnityEngine;

namespace _Project.Code.Runtime.Gameplay.Character.View
{
    public class PreviewCharacterView : CharacterView
    {
        [Header("StandZone")]
        [SerializeField] private Transform _highlightStandZone;
        
        [Header("ReadyMassage")]
        [SerializeField] private RectTransform _readyLable;
        
        public void ToggleStandZone(bool enable) => 
            _highlightStandZone.gameObject.SetActive(enable);

        public override void SwitchViewBaseOnRole(GameRole role)
        {
            base.SwitchViewBaseOnRole(role);
            UpdateReadyMassagePositionBaseOnView(role);
        }

        public void ToggleReadyLable(bool value) => 
            _readyLable.gameObject.SetActive(value);
        
        public void UpdateReadyMassagePositionBaseOnView(GameRole role)
        {
            _readyLable.anchoredPosition = role == GameRole.Hider ?
                new(_readyLable.anchoredPosition.x, 0.35f) :
                new(_readyLable.anchoredPosition.x, 0.64f); //constants
        }
    }
}