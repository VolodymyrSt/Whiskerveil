using System;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.Gameplay.Character.View;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Code.Runtime.Gameplay.Character
{
    public interface ICharacter
    {
        void AssignRole(GameRole role);

        Transform Transform { get; }
        GameRole Role { get; }
        ICharacterView View { get; }
        Transform Head { get; }
        Transform CameraHolder { get; }
        void AllowJump(bool allow);
        void AllowMove(bool allow);
        void AllowAttack(bool allow);
        void AllowLook(bool allow);
        event Action<ulong> OnSeekerKilled;
        void Teleport(Vector3 position);
    }
}