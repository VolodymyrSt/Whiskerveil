using System;
using _Project.Code.Runtime.CommonServices.InputManagement;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.StaticData;
using _Project.Code.Runtime.Gameplay.Camera;
using _Project.Code.Runtime.Gameplay.Camera.Factory;
using _Project.Code.Runtime.Gameplay.Character.View;
using _Project.Code.Runtime.Infrustructure.AssetsManagement;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Object = System.Object;

namespace _Project.Code.Runtime.Gameplay.Character
{
    public class Character : NetworkBehaviour, ICharacter
    {
        [Header("Base")]
        [SerializeField] private CharacterView _view;
        
        [Header("Kinematic")]
        [SerializeField] private CharacterController _characterController;
        
        [Header("For Camera")]
        [SerializeField] private Transform _head;
        [SerializeField] private Transform _cameraHolder;
        
        private readonly NetworkVariable<FixedString64Bytes> _name = new();
        private readonly NetworkVariable<GameRole> _role = new();
        
        public Transform Transform => transform;
        public ICharacterView View => _view;
        public GameRole Role => _role.Value;
        public Transform Head => _head;
        public Transform CameraHolder => _cameraHolder;

        private IStaticDataService _staticDataService;
        private ICameraFactory _cameraFactory;
        private IInputService _input;
        private IFPVCameraHandler _fpvCamera;
        private CharacterMover _mover;
        
        private bool _cameraInitialized;
        private bool _jumpBuffered;
        private float _jumpBufferTime = 0.15f;
        private float _jumpBufferTimer;

        [Inject]
        private void Construct(IStaticDataService staticDataService, ICameraFactory cameraFactory)
        {
            _staticDataService = staticDataService;
            _cameraFactory = cameraFactory;
        }

        private void OnValidate()
        {
            _view ??= GetComponentInChildren<CharacterView>();
            _characterController ??= GetComponentInChildren<CharacterController>();
        }
        
        public override void OnNetworkSpawn()
        {
            if (_staticDataService == null) //bad
                 ProjectContext.Instance.Container.Inject(this);
            
            _role.OnValueChanged += SwitchViewBaseOnRole;
            _name.OnValueChanged += ChangeName;

            SwitchViewBaseOnRole(_role.Value, _role.Value);
            ChangeName(_name.Value, _name.Value);

            if (IsServer)
            {
                _role.OnValueChanged += InitMoverBaseOnRole;
                
                if (_role.Value != GameRole.None)
                    InitMover(_role.Value);
            }

            if (IsOwner)
            {
                _input = new InputService();
                _input.Enable();
                _view.HideNick();

                _fpvCamera = _cameraFactory.CreateCamera(_cameraHolder);
                _fpvCamera.Init(this, _input, _staticDataService);
                _cameraInitialized = true;
            }
        }
        
        private void Update()
        {
            if (!IsOwner) return;
            
            if (_input.PlayerJumpPressed())
            {
                _jumpBuffered = true;
                _jumpBufferTimer = _jumpBufferTime;
            }
    
            if (_jumpBufferTimer > 0f)
                _jumpBufferTimer -= Time.deltaTime;
            else
                _jumpBuffered = false;
            
            MoveServerRpc(
                _input.GetCharacterMoveVector(),
                _input.PlayerJumpPressed(),
                _input.PlayerSprintPressed(),
                _fpvCamera.Rotation,
                Time.deltaTime
            );
            
            if (_jumpBuffered && _jumpBufferTimer <= 0f)
                _jumpBuffered = false;
        }

        private void LateUpdate()
        {
            if (!IsOwner) return;
            if (!_cameraInitialized) return;
            _fpvCamera.LateTick();
        }

        public void SetName(string characterName)
        {
            if (!IsServer) return;
            _name.Value = characterName;
        }

        public void AllowJump(bool allow) =>
            _mover.ToggleJump(allow);
        
        [ServerRpc]
        private void MoveServerRpc(Vector2 moveInput, bool jumpInput, bool sprintInput, Quaternion cameraRotation, float deltaTime)
        {
            deltaTime = Mathf.Clamp(deltaTime, 0f, 0.05f);
            _mover.ProcessInput(moveInput, cameraRotation, jumpInput, sprintInput);
            _mover.UpdateVelocity(deltaTime);
            _mover.UpdateRotation(deltaTime);
        }

        private void ChangeName(FixedString64Bytes old, FixedString64Bytes @new)
        {
            _view.UpdateName(@new.ToString());
            _view.UpdateNicknamePositionBaseOnView(_role.Value);
        }

        public void AssignRole(GameRole role)
        {
            if (!IsServer) return;
            _role.Value = role;
        }
        
        private void SwitchViewBaseOnRole(GameRole oldRole, GameRole newRole) => 
            _view.SwitchViewBaseOnRole(newRole);
        
        private void InitMoverBaseOnRole(GameRole oldRole, GameRole newRole) => InitMover(newRole);

        private void InitMover(GameRole role)
        {
            _mover = new CharacterMover(
                _characterController,
                _staticDataService.GetCharacterConfigForRole(role),
                _head);
    
            _mover.Init();
        }

        public override void OnNetworkDespawn()
        {
            _role.OnValueChanged -= SwitchViewBaseOnRole;
            _name.OnValueChanged -= ChangeName;
            _role.OnValueChanged -= InitMoverBaseOnRole;
            _input?.Disable();
        }
    }
}
