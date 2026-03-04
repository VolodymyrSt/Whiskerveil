using System.Collections.Generic;
using _Project.Code.Runtime.Character.View;
using _Project.Code.Runtime.CommonServices.RolePicker;
using UnityEngine;

namespace _Project.Code.Runtime.Character
{
    public interface ICharacter
    {
        void AssignRole(GameRole role);

        Transform Transform { get; }
        GameRole Role { get; }
    }
}