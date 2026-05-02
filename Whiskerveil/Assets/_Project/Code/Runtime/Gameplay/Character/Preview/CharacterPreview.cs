using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.Gameplay.Character.View;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Code.Runtime.Gameplay.Character.Preview
{
    public class CharacterPreview : NetworkBehaviour, IPreview
    {
        [Header("Base")]
        [SerializeField] private PreviewCharacterView _view;
        
        private readonly NetworkVariable<FixedString64Bytes> _name = new NetworkVariable<FixedString64Bytes>();
        private readonly NetworkVariable<GameRole> _role = new NetworkVariable<GameRole>();
        private readonly NetworkVariable<bool> _isReadyInLobby = new NetworkVariable<bool>(false);
        
        public Transform Transform => transform;
        public ICharacterView View => _view;
        public GameRole Role => _role.Value;
        
        private void OnValidate() => 
            _view ??= GetComponentInChildren<PreviewCharacterView>();
        
        public override void OnNetworkSpawn()
        {
            _role.OnValueChanged += SwitchViewBaseOnRole;
            _name.OnValueChanged += ChangeName;
            _isReadyInLobby.OnValueChanged += OnReadinessInLobbyChanged;

            SwitchViewBaseOnRole(_role.Value, _role.Value);
            ChangeName(_name.Value, _name.Value);
            OnReadinessInLobbyChanged(_isReadyInLobby.Value, _isReadyInLobby.Value);
            
            if (IsOwner)
                _view.ToggleStandZone(enable: true);
        }
        
        public void SetReadyInLobby(bool ready)
        {
            if (!IsServer) return;
            _isReadyInLobby.Value = ready;
        }
        
        public void SetName(string characterName)
        {
            if (!IsServer) return;
            _name.Value = characterName;
        }

        private void ChangeName(FixedString64Bytes old, FixedString64Bytes @new)
        {
            _view.UpdateName(@new.ToString());
            _view.UpdateNicknamePositionBaseOnView(_role.Value);
            _view.UpdateReadyMassagePositionBaseOnView(_role.Value);
        }

        public void AssignRole(GameRole role)
        {
            if (!IsServer) return;
            _role.Value = role;
        }
        
        private void SwitchViewBaseOnRole(GameRole oldRole, GameRole newRole) => 
            _view.SwitchViewBaseOnRole(newRole);

        private void OnReadinessInLobbyChanged(bool previousValue, bool newValue) => 
            _view.ToggleReadyLable(newValue);
        
        public override void OnNetworkDespawn()
        {
            _role.OnValueChanged -= SwitchViewBaseOnRole;
            _name.OnValueChanged -= ChangeName;
            _isReadyInLobby.OnValueChanged -= OnReadinessInLobbyChanged;
        }
    }
}