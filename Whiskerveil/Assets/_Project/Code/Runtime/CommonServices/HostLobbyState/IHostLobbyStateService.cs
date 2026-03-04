using _Project.Code.Runtime.Character;

namespace _Project.Code.Runtime.CommonServices.HostLobbyState
{
    public interface IHostLobbyStateService
    {
        void BindPlayerToSlot(ulong clientId, ICharacter player);
        void RemoveSlot(ulong clientId);
        void Clear();
        void ApplyPlayerToSlot(ulong clientId, ICharacter player);
        bool HasState { get; }
    }
}