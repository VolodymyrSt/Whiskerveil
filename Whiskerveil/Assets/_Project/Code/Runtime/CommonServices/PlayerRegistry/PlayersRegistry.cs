using System;
using System.Collections.Generic;

namespace _Project.Code.Runtime.CommonServices.PlayerRegistry
{
    public class PlayersRegistry : IPlayersRegistry
    {
        public event Action<PlayerProfile> OnNewPlayerAdded;
        
        private readonly List<PlayerProfile> _profiles = new List<PlayerProfile>();
        
        public List<PlayerProfile> Profiles => _profiles;
        public bool IsEmpty => _profiles.Count == 0;

        public void AddProfile(PlayerProfile playerProfile)
        {
            _profiles.Add(playerProfile);
            OnNewPlayerAdded?.Invoke(playerProfile);
        }

        public void RemoveProfile(ulong id)
        {
            PlayerProfile profile = _profiles.Find(x => x.Id == id);
            
            if (profile == null)
                throw new Exception("Profile not found for id: " + id);
                
            _profiles.Remove(profile);
        }

        public PlayerProfile GetById(PlayerProfile player) =>
            _profiles.Find(x => x.Id == player.Id);
        
        public void Clear() => 
            _profiles.Clear();
    }
}