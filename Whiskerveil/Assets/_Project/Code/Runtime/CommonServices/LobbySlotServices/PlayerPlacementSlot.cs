using _Project.Code.Runtime.CommonServices.RolePickerService;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Code.Runtime.CommonServices.LobbySlotServices
{
    public class PlayerPlacementSlot : NetworkBehaviour
    {
        [SerializeField] private GameRole _forRole;
        
        private readonly NetworkVariable<ulong> _occupiedBy = new NetworkVariable<ulong>(0);
        
        public bool IsOccupied => _occupiedBy.Value != 0;
        public GameRole ForRole => _forRole;
        public ulong PlayerId => _occupiedBy.Value;

        public void Occupy(ulong playerId)
        {
            if (IsOccupied) return;
            _occupiedBy.Value = playerId;

        }

        public void UnOccupy(ulong playerId)
        {
            if (_occupiedBy.Value != playerId) return;
            _occupiedBy.Value = 0;
        }
    }
}