using _Project.Code.Runtime.CommonServices.RolePicker;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Code.Runtime.CommonServices.TimeManagement
{
    public class CountdownTimerView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private Image _roleImage;
        
        [SerializeField] private Sprite _mouseSprite;
        [SerializeField] private Sprite _catSprite;
        
        public void SetIconBaseOnRole(GameRole role) => //Fix
            _roleImage.sprite = role == GameRole.Hider ? _mouseSprite: _catSprite;

        public void UpdateTimerText(float remaining) => 
            _timerText.text = Mathf.CeilToInt(remaining).ToString() + "sec";
    }
}