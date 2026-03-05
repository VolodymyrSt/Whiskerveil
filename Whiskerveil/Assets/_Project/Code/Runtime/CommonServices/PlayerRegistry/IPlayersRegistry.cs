using System;
using System.Collections.Generic;

namespace _Project.Code.Runtime.CommonServices.PlayerRegistry
{
    public interface IPlayersRegistry
    {
        void AddProfile(PlayerProfile playerProfile);
        PlayerProfile GetById(PlayerProfile player);
        void Clear();
        void RemoveProfile(ulong id);
        List<PlayerProfile> Profiles { get; }
        bool IsEmpty { get; }
        event Action<PlayerProfile> OnNewPlayerAdded;
    }
}