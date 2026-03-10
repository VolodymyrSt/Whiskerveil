using System;
using _Project.Code.Runtime.Character.View;
using _Project.Code.Runtime.CommonServices.RolePicker;
using UnityEngine;

namespace _Project.Code.Runtime.Character
{
    [Serializable]
    public struct ViewByRoleData
    {
        public GameRole Role;
        public GameObject View;
    }
}