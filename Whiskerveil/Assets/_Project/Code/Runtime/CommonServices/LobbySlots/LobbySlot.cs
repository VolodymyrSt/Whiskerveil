using _Project.Code.Runtime.CommonServices.RolePicker;
using UnityEngine;

namespace _Project.Code.Runtime.CommonServices.LobbySlots
{
    public class LobbySlot
    {
        public string Id;
        public ulong PlayerId;
        public GameRole ForRole;
        public Vector3 Position;
        public Quaternion Rotation;
        public bool IsTaken;

        public LobbySlot(string id, GameRole forRole, Vector3 position, Quaternion rotation)
        {
            Id = id;
            ForRole = forRole;
            Position = position;
            Rotation = rotation;
        }
    }
}