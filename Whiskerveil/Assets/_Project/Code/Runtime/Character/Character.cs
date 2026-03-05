using System.Collections.Generic;
using _Project.Code.Runtime.Character.Factory;
using _Project.Code.Runtime.Character.View;
using _Project.Code.Runtime.CommonServices.LobbySlots;
using _Project.Code.Runtime.CommonServices.RolePicker;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Character
{
    public class Character : NetworkBehaviour, ICharacter
    {
        [SerializeField] private List<ViewByRoleData> _viewByRoles = new List<ViewByRoleData>();
        
        private readonly NetworkVariable<GameRole> _role = new NetworkVariable<GameRole>();
        
        private ICharacterView _view;
        private ulong _id;
        
        public Transform Transform => transform;
        public GameRole Role => _role.Value;
        public ulong Id => _id;

        public override void OnNetworkSpawn()
        {
            _role.OnValueChanged += SwitchViewBaseOnRole;

            SwitchViewBaseOnRole(_role.Value, _role.Value);
        }

        public void AssignRole(GameRole role)
        {
            if (!IsServer) return;

            _role.Value = role;
        }
        
        public void SetId(ulong id) => 
            _id = id;
        
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
        }
    }
}