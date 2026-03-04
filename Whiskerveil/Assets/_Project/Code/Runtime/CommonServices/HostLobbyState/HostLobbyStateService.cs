using System.Collections.Generic;
using _Project.Code.Runtime.Character;
using _Project.Code.Runtime.CommonServices.LobbySlots;

namespace _Project.Code.Runtime.CommonServices.HostLobbyState
{
    public class HostLobbyStateService : IHostLobbyStateService
    {
        private readonly Dictionary<ulong, string> _clientToSlot = new();
        private readonly ILobbySlotService _slotService;
        
        public bool HasState => _clientToSlot.Count > 0;

        public HostLobbyStateService(ILobbySlotService slotService) => 
            _slotService = slotService;

        public void ApplyPlayerToSlot(ulong clientId, ICharacter player)
        {
            if (_clientToSlot.TryGetValue(clientId, out string slotId))
            {
                if (_slotService.TrySlotById(slotId, out LobbySlot slot))
                {
                    player.Transform.position = slot.Position;
                    player.Transform.rotation = slot.Rotation;
                }
            }
        }
        
        public void BindPlayerToSlot(ulong clientId, ICharacter player)
        {
            if (_slotService.TryGetFreeSlotFor(player.Role, out LobbySlot slot))
            {
                _clientToSlot[clientId] = slot.Id;
                slot.IsTaken = true;
                player.Transform.position = slot.Position;
                player.Transform.rotation = slot.Rotation;
            }
        }

        public void RemoveSlot(ulong clientId)
        {
            _clientToSlot.Remove(clientId);
        }

        public void Clear() => 
            _clientToSlot.Clear();
    }
}