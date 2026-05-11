

using UnityEngine;

namespace _Project.Code.Runtime.CommonServices.InputManagement
{
    public interface IInputService
    {
        void Enable();
        void Disable();
        bool PlayerJumpPressed();
        bool PlayerSprintPressed();
        Vector2 GetCharacterMoveVector();
        Vector2 GetCharacterLookVector();
        bool PlayerAttackPressed();
    }
}