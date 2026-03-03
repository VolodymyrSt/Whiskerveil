using System;
using System.Collections.Generic;
using _Project.Code.Runtime.CommonServices.RolePickerService;

namespace _Project.Code.Runtime.CommonServices.LobbySlotServices
{
    public class LobbySlotService : ILobbySlotService
    {
        private readonly List<PlayerPlacementSlot> _slots;
        
        public LobbySlotService(List<PlayerPlacementSlot> slots) => 
            _slots = slots;

        public bool TryGetFreeSlotFor(GameRole role, out PlayerPlacementSlot slot)
        {
            slot = _slots.Find(x => !x.IsOccupied && x.ForRole == role);
            return slot != null;
        }
    }
}