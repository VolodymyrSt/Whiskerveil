using _Project.Code.Runtime.CommonServices.RolePicker;

namespace _Project.Code.Runtime.CommonServices.LobbySlots
{
    public interface ILobbySlotService
    {
        bool TryGetFreeSlotFor(GameRole role, out LobbySlot slot);
        void PrepareSlots();
        bool TrySlotById(string id, out LobbySlot slot);
    }
}