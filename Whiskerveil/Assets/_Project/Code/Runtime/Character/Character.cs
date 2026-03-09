using System.Collections.Generic;
using _Project.Code.Runtime.Character.Factory;
using _Project.Code.Runtime.Character.View;
using _Project.Code.Runtime.CommonServices.LobbySlots;
using _Project.Code.Runtime.CommonServices.RolePicker;
using Sirenix.Utilities;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Character
{
    public class Character : NetworkBehaviour, ICharacter
    {
        [SerializeField] private List<ViewByRoleData> _viewByRoles = new List<ViewByRoleData>();
        
        private readonly NetworkVariable<GameRole> _role = new NetworkVariable<GameRole>();
        private NetworkVariable<FixedString64Bytes> _name = new NetworkVariable<FixedString64Bytes>();

        [Header("Nickname")]
        [SerializeField] private RectTransform _nicknameRoot;
        [SerializeField] private TextMeshProUGUI _nickname;
        
        private ICharacterView _view;

        public Transform Transform => transform;
        public GameRole Role => _role.Value;
        public ulong Id => OwnerClientId;//bad
        
        public void SetName(string name)
        {
            if (!IsServer) return;
            _name.Value = name;
        }

        public override void OnNetworkSpawn()
        {
            _role.OnValueChanged += SwitchViewBaseOnRole;
            _name.OnValueChanged += ChangeName;

            SwitchViewBaseOnRole(_role.Value, _role.Value);
            ChangeName(_name.Value, _name.Value);
        }

        private void ChangeName(FixedString64Bytes old, FixedString64Bytes @new)
        {
            _nickname.text = @new.ToString();
            UpdateNicknamePositionBaseOnView();
        }

        public void AssignRole(GameRole role)
        {
            if (!IsServer) return;
            _role.Value = role;
        }
        
        private void SwitchViewBaseOnRole(GameRole oldRole, GameRole newRole)
        {
            foreach (var data in _viewByRoles)
            {
                if (data.Role == newRole)
                {
                    _view = data.View;
                    _view.Toggle(true);
                }
                else
                {
                    data.View.Toggle(false);
                }
            }
            
            UpdateNicknamePositionBaseOnView();
        }
        
        private void UpdateNicknamePositionBaseOnView()
        {
            _nicknameRoot.anchoredPosition = _role.Value == GameRole.Hider ?
                new(_nicknameRoot.anchoredPosition.x, 0.8f) :
                new(_nicknameRoot.anchoredPosition.x, 1.5f); //constants
        }

        public override void OnNetworkDespawn()
        {
            _role.OnValueChanged -= SwitchViewBaseOnRole;
            _name.OnValueChanged -= ChangeName;
        }
    }
}
