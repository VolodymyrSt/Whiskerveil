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
        
        public void SetRileIcon(Sprite sprite) =>
            _roleImage.sprite = sprite;
        
        public void UpdateTimerText(float remaining) => 
            _timerText.text = Mathf.CeilToInt(remaining).ToString() + "sec";
    }
}