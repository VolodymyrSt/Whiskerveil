using System.Collections.Generic;
using _Project.Code.Runtime.Character;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.Configs.Lobby;
using Unity.Netcode;

namespace _Project.Code.Runtime.CommonServices.LobbySlots
{
    public class LobbySlotService : ILobbySlotService
    {
        private readonly List<LobbySlot> _slots = new();
        private readonly LobbySlotsDataHolder _slotsDataDataHolder;
        
        public LobbySlotService(LobbySlotsDataHolder lobbySlotsDataDataHolder) => 
            _slotsDataDataHolder = lobbySlotsDataDataHolder;

        public void PrepareSlots()
        {
            if (!NetworkManager.Singleton.IsServer) return;
            
            _slots.Clear();

            foreach (var slotData in _slotsDataDataHolder.Slots)
                _slots.Add(new LobbySlot(slotData.Id, slotData.ForRole, slotData.Position, slotData.Rotation));
        }
        
        public LobbySlot GetFreeSlotFor(GameRole role)
        {
            LobbySlot foundSlot = _slots.Find(x => !x.IsTaken && x.ForRole == role);

            if (foundSlot != null)
                return foundSlot;
            
            throw new KeyNotFoundException($"LobbySlot with role {role} not available");
        }

        public LobbySlot GetSlotById(string id)
        {
            LobbySlot foundSlot = _slots.Find(x => x != null && x.Id == id);

            if (foundSlot != null)
                return foundSlot;
            
            throw new KeyNotFoundException($"LobbySlot with id {id} not found");
        }
    }
}