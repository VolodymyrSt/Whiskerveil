using _Project.Code.Runtime.Configs.Character;
using _Project.Code.Runtime.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Code.Runtime.Gameplay.Character
{
    public class CharacterMover
    {
        private readonly CharacterController _controller;
        private readonly Transform _head;
        
        private readonly float _jumpForce;
        
        private readonly float _walkSpeed;
        private readonly float _sprintSpeed;
        
        private readonly float _walkResponse;
        private readonly float _sprintResponse;
        
        private readonly float _standCameraTargetHeight;
        private readonly float _jumpCooldown;
        
        private readonly float _standHeight;
        private readonly float _colliderRadius;
        private readonly float _colliderYOffset;
        
        private readonly float _airAcceleration;
        private readonly float _airSpeed;
        
        private readonly float _groundCheckDistance;
        private readonly LayerMask _groundMask;
        
        private Vector3 _currentVelocity;
        private float _verticalVelocity;
        
        private Vector3 _requestedMoveDirection;
        private Quaternion _requestedRotation;
        private bool _requestedJumpInput;
        private bool _requestedSprintInput;
        
        private bool _isJumpAllowed = true;
        private bool _isMoveAllowed = true;
        private bool _isGrounded;
        
        public CharacterMover(CharacterController controller, CharacterConfigSO config, Transform head)
        {
            _controller = controller;
            _head = head;
            
            _walkSpeed                = config.WalkSpeed;
            _sprintSpeed              = config.SprintSpeed;
            _jumpForce                = config.JumpForce;
            _standCameraTargetHeight  = config.StandCameraTargetHeight;
            _standHeight              = config.StandHeight;
            _walkResponse             = config.WalkResponse;
            _sprintResponse           = config.SprintResponse;
            _airAcceleration          = config.AirAcceleration;
            _airSpeed                 = config.AirSpeed;
            _colliderRadius           = config.ColliderRadius;
            _colliderYOffset          = config.YOffset;
            _groundCheckDistance      = config.GroundCheckDistance;
            _groundMask               = config.GroundMask;
        }

        public void Init()
        {
            SetCapsuleDimensionsByStance(_standHeight, _colliderRadius, _colliderYOffset);
            _head.localPosition = new Vector3(_head.localPosition.x, _standCameraTargetHeight, _head.localPosition.z);
        }
        
        public void ToggleJump(bool allow) => 
            _isJumpAllowed = allow;
        
        public void ToggleMove(bool allow) => 
            _isMoveAllowed = allow;

        public void ProcessInput(Vector3 moveInput, Quaternion rotation, bool jumpInput, bool sprintInput)
        {
            _requestedRotation      = rotation;
            _requestedMoveDirection = rotation * new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            _requestedSprintInput   = sprintInput;
            
            if (jumpInput) 
                _requestedJumpInput = true;
        }
        
        public void UpdateRotation(float deltaTime)
        {
            var lookDirection = Vector3.ProjectOnPlane(_requestedRotation * Vector3.forward, Vector3.up);
    
            if (lookDirection != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                _controller.transform.rotation = Quaternion.Slerp(
                    _controller.transform.rotation,
                    targetRotation,
                    1f - Mathf.Exp(-_walkResponse * deltaTime));
            }
        }

        public void UpdateVelocity(float deltaTime)
        {
            _isGrounded = IsOnGround(); 
            
            ApplyGravity(deltaTime);
    
            if (_isMoveAllowed)
                PerformMove(deltaTime);

            if (CanJump())
                PerformJump();
        }
        
        private void ApplyGravity(float deltaTime)
        {
            if (_isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -1f;
            else
                _verticalVelocity += Constants.Gravity * deltaTime;

            _controller.Move(Vector3.up * _verticalVelocity * deltaTime);
        }
        
        private void PerformMove(float deltaTime)
        {
            if (_isGrounded)
            {
                var speed    = _requestedSprintInput ? _sprintSpeed : _walkSpeed;
                var response = _requestedSprintInput ? _sprintResponse : _walkResponse;

                _currentVelocity = Vector3.Lerp(
                    _currentVelocity,
                    _requestedMoveDirection * speed,
                    1f - Mathf.Exp(-response * deltaTime));
            }
            else
            {
                _currentVelocity = Vector3.Lerp(
                    _currentVelocity,
                    _requestedMoveDirection * _airSpeed,
                    1f - Mathf.Exp(-_airAcceleration * deltaTime));
            }

            _controller.Move(_currentVelocity * deltaTime);
        }

        private void PerformJump()
        {
            _verticalVelocity = Mathf.Sqrt(_jumpForce * -2f * Constants.Gravity);
            _requestedJumpInput = false;
            _isGrounded         = false; 
        }

        private void SetCapsuleDimensionsByStance(float height, float radius, float yOffset)
        {
            _controller.height = height;
            _controller.radius = radius;
            _controller.center = new Vector3(_controller.center.x, yOffset, _controller.center.z);
        }

        private bool CanJump() => _isJumpAllowed && _requestedJumpInput && _isGrounded;
        
        private bool IsOnGround()
        {
            var origin = _controller.transform.position + Vector3.up * _controller.radius;
    
            return Physics.SphereCast(
                origin,
                0.02f, 
                Vector3.down,
                out _,
                _groundCheckDistance + _controller.radius,
                _groundMask,
                QueryTriggerInteraction.Ignore);
        }
    }
}