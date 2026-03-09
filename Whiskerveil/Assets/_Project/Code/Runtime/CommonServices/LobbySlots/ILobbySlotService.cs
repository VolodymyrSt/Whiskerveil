using _Project.Code.Runtime.CommonServices.RolePicker;
using Unity.Collections;

namespace _Project.Code.Runtime.CommonServices.LobbySlots
{
    public interface ILobbySlotService
    {
        LobbySlot GetFreeSlotFor(GameRole role);
        void PrepareSlots();
        LobbySlot GetSlotById(FixedString64Bytes id);
    }
}