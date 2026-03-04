using System;
using _Project.Code.Runtime.Character.View;
using _Project.Code.Runtime.CommonServices.RolePicker;

namespace _Project.Code.Runtime.Character
{
    [Serializable]
    public struct ViewByRoleData
    {
        public GameRole Role;
        public CharacterView View;
    }
}