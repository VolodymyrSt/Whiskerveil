using _Project.Code.Runtime.CommonServices.RolePicker;
using Unity.Collections;
using UnityEngine;

namespace _Project.Code.Runtime.CommonServices.SlotsManagement
{
    public class LobbySlot : BaseSlot
    {
        public FixedString64Bytes Id;

        public LobbySlot(FixedString64Bytes id, GameRole forRole, Vector3 position, Quaternion rotation) 
            : base(forRole, position, rotation)
        {
            Id = id;
            IsTaken = false;
        }
    }

    public class BaseSlot
    {
        public GameRole ForRole;
        public Vector3 Position;
        public Quaternion Rotation;
        public bool IsTaken;
        
        public BaseSlot(GameRole forRole, Vector3 position, Quaternion rotation)
        {
            ForRole = forRole;
            Position = position;
            Rotation = rotation;
            IsTaken = false;
        }
    }
}