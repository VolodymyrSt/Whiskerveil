using System;
using System.Collections.Generic;
using _Project.Code.Runtime.CommonServices.RolePicker;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Code.Runtime.Character.View
{
    public class CharacterView : MonoBehaviour, ICharacterView
    {
        [SerializeField] private List<ViewByRoleData> _viewByRoles = new List<ViewByRoleData>();
        
        private readonly NetworkVariable<bool> _isReadyInLobby = new NetworkVariable<bool>(false);

        [Header("Nickname")]
        [SerializeField] private RectTransform _nicknameRoot;
        [SerializeField] private TextMeshProUGUI _nickname;
        
        [Header("ReadyMassage")]
        [SerializeField] private RectTransform _readyLable;
        
        public void UpdateName(string characterName) => 
            _nickname.text = characterName;

        public void SwitchViewBaseOnRole(GameRole role)
        {
            foreach (var data in _viewByRoles)
            {
                if (data.Role == role)
                    data.View.gameObject.SetActive(true);
                else
                    data.View.gameObject.SetActive(false);
            }
            
            UpdateNicknamePositionBaseOnView(role);
            UpdateReadyMassagePositionBaseOnView(role);
        }

        public void ToggleReadyLable(bool value) => 
            _readyLable.gameObject.SetActive(value);
        
        public void UpdateNicknamePositionBaseOnView(GameRole role)
        {
            _nicknameRoot.anchoredPosition = role == GameRole.Hider ?
                new(_nicknameRoot.anchoredPosition.x, 0.8f) :
                new(_nicknameRoot.anchoredPosition.x, 1.8f); //constants
        }

        public void UpdateReadyMassagePositionBaseOnView(GameRole role)
        {
            _readyLable.anchoredPosition = role == GameRole.Hider ?
                new(_readyLable.anchoredPosition.x, 0.6f) :
                new(_readyLable.anchoredPosition.x, 1.6f); //constants
        }
    }
}
