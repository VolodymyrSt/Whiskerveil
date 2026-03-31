using _Project.Code.Runtime.CommonServices.RolePicker;
using Unity.Collections;

namespace _Project.Code.Runtime.CommonServices.SlotsManagement
{
    public interface ISlotService
    {
        LobbySlot GetFreeLobbySlotFor(GameRole role);
        void PrepareLobbySlots();
        LobbySlot GetLobbySlotById(FixedString64Bytes id);
        BaseSlot GetFreeLevelSlotFor(GameRole role);
        void PrepareLevelSlots();
        void Initialize();
    }
}