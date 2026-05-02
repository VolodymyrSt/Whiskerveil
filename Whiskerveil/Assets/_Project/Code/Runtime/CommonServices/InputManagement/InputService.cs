

using UnityEngine;

namespace _Project.Code.Runtime.CommonServices.InputManagement
{
    public class InputService : IInputService
    {
        private readonly Player_Input_Action _inputAction = new();
        
        public void Enable() => 
            _inputAction.Enable();
        
        public void Disable() => 
            _inputAction.Disable();
        
        public bool PlayerJumpPressed() =>
            _inputAction.Player.Jump.IsPressed();
        
        public bool PlayerSprintPressed() =>
            _inputAction.Player.Sprint.IsPressed();
        
        public Vector2 GetCharacterMoveVector() => 
            _inputAction.Player.Move.ReadValue<Vector2>();
        
        public Vector2 GetCharacterLookVector() => 
            _inputAction.Player.Look.ReadValue<Vector2>();
    }
}