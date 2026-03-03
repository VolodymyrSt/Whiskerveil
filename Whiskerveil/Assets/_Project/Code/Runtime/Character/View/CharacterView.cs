using TMPro;
using Unity.Netcode;
using UnityEngine;
using Zenject.SpaceFighter;

namespace _Project.Code.Runtime.Character.View
{
    public class CharacterView : MonoBehaviour, ICharacterView
    {
        [SerializeField] private Canvas _nicknameCanvas;
        [SerializeField] private TextMeshProUGUI _nicknameText;

        public void SetNickname(string nickname)
        {
            _nicknameText.text = nickname;
        }
        
        public void Toggle(bool value) => 
            gameObject.SetActive(value);
    }
}