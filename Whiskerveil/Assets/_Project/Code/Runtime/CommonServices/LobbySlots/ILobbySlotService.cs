using _Project.Code.Runtime.CommonServices.RolePicker;

namespace _Project.Code.Runtime.CommonServices.LobbySlots
{
    public interface ILobbySlotService
    {
        LobbySlot GetFreeSlotFor(GameRole role);
        void PrepareSlots();
        LobbySlot GetSlotById(string id);
    }
}