using _Project.Code.Runtime.CommonServices.RolePicker;
using Unity.Collections;
using UnityEngine;

namespace _Project.Code.Runtime.CommonServices.LobbySlots
{
    public class LobbySlot
    {
        public FixedString64Bytes Id;
        public GameRole ForRole;
        public Vector3 Position;
        public Quaternion Rotation;
        public bool IsTaken;

        public LobbySlot(FixedString64Bytes id, GameRole forRole, Vector3 position, Quaternion rotation)
        {
            Id = id;
            ForRole = forRole;
            Position = position;
            Rotation = rotation;
        }
    }
}