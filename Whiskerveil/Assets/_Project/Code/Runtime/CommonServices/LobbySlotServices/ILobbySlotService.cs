using _Project.Code.Runtime.CommonServices.RolePickerService;

namespace _Project.Code.Runtime.CommonServices.LobbySlotServices
{
    public interface ILobbySlotService
    {
        bool TryGetFreeSlotFor(GameRole role, out PlayerPlacementSlot slot);
    }
}