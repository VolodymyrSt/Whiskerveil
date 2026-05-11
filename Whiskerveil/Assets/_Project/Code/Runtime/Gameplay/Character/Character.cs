using System;
using _Project.Code.Runtime.CommonServices.GameState;
using _Project.Code.Runtime.CommonServices.InputManagement;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.StaticData;
using _Project.Code.Runtime.Configs.Character;
using _Project.Code.Runtime.Gameplay.Camera;
using _Project.Code.Runtime.Gameplay.Camera.Factory;
using _Project.Code.Runtime.Gameplay.Character.Modules;
using _Project.Code.Runtime.Gameplay.Character.View;
using _Project.Code.Runtime.Infrustructure.AssetsManagement;
using _Project.Code.Runtime.Utils;
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
        
        [Header("Attack")]
        [SerializeField] private Transform _attackPoint;
        
        private readonly NetworkVariable<FixedString64Bytes> _name = new();
        private readonly NetworkVariable<GameRole> _role = new();
        
        public Transform Transform => transform;
        public ICharacterView View => _view;
        public GameRole Role => _role.Value;
        public Transform Head => _head;
        public Transform CameraHolder => _cameraHolder;

        private IStaticDataService _staticDataService;
        private IGameStateService _gameStateService;
        private ICameraFactory _cameraFactory;
        private IInputService _input;
        private IFPVCameraHandler _fpvCamera;
        private CharacterMover _mover;
        private AttackModule _attackModule;
        
        private bool _cameraInitialized;

        [Inject]
        private void Construct(IStaticDataService staticDataService, ICameraFactory cameraFactory
            , IGameStateService gameStateService)
        {
            _staticDataService = staticDataService;
            _cameraFactory = cameraFactory;
            _gameStateService = gameStateService;
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
                _role.OnValueChanged += InitAttackModuleBaseOnRole;

                if (_role.Value != GameRole.None)
                {
                    InitMover(_role.Value);
                    InitAttackModule(_role.Value);
                }
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
            
            MoveServerRpc(
                _input.GetCharacterMoveVector(),
                _input.PlayerJumpPressed(),
                _input.PlayerSprintPressed(),
                _fpvCamera.Rotation,
                Time.deltaTime
            );

            if (_input.PlayerAttackPressed())
                AttackServerRpc();
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
        
        public void AllowMove(bool allow) =>
            _mover.ToggleMove(allow);
        
        [ServerRpc]
        private void MoveServerRpc(Vector2 moveInput, bool jumpInput, bool sprintInput, Quaternion cameraRotation, float deltaTime)
        {
            deltaTime = Mathf.Clamp(deltaTime, 0f, 0.05f);
            _mover.ProcessInput(moveInput, cameraRotation, jumpInput, sprintInput);
            _mover.UpdateVelocity(deltaTime);
            _mover.UpdateRotation(deltaTime);
            
            UpdateAnimationClientRpc(
                moveInput.magnitude > Constants.Epsilon,
                jumpInput,
                sprintInput
            );
        }
        
        [ServerRpc]
        private void AttackServerRpc()
        {
            if (_role.Value != GameRole.Seeker)
                return;

            if (_attackModule.TryAttack(out var hiderId))
            {
                Debug.Log("Id");
                _gameStateService.SetClientGameplayStateToDead(hiderId);
            }
        }

        [ClientRpc]
        private void UpdateAnimationClientRpc(bool isWalking, bool jump, bool sprinting)
        {
            if (IsOwner) return;
            _view.UpdateAnimation(isWalking, sprinting, jump);
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
        private void InitAttackModuleBaseOnRole(GameRole oldRole, GameRole newRole) => InitAttackModule(newRole);

        private void InitMover(GameRole role)
        {
            _mover = new CharacterMover(
                _characterController,
                _staticDataService.GetCharacterConfigForRole(role),
                _head);
    
            _mover.Init();
        }
        
        private void InitAttackModule(GameRole role)
        {
            if (role != GameRole.Seeker)
                return;

            CharacterConfigSO config =
                _staticDataService.GetCharacterConfigForRole(GameRole.Seeker);

            _attackModule = new AttackModule(
                _attackPoint,
                config.AttackRadius,
                config.AttackMask,
                config.AttackCooldown);
        }

        public override void OnNetworkDespawn()
        {
            _role.OnValueChanged -= SwitchViewBaseOnRole;
            _name.OnValueChanged -= ChangeName;
            _role.OnValueChanged -= InitMoverBaseOnRole;
            _role.OnValueChanged -= InitAttackModuleBaseOnRole;
            _input?.Disable();
        }
    }
}
