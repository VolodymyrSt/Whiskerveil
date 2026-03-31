using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Code.Runtime.CommonServices.RolePicker;

namespace _Project.Code.Runtime.CommonServices.ClientRegistry
{
    public class ClientsRegistry : IClientsRegistry
    {
        public event Action<ClientProfile> OnNewClientAdded;
        public event Action<ulong> OnClientRemoved;
        
        private readonly List<ClientProfile> _profiles = new List<ClientProfile>();
        
        public List<ClientProfile> Profiles => _profiles;
        public bool IsEmpty => _profiles.Count == 0;
        public int TotalCount => _profiles.Count;

        public void AddProfile(ClientProfile clientProfile)
        {
            _profiles.Add(clientProfile);
            OnNewClientAdded?.Invoke(clientProfile);
        }

        public void RemoveProfile(ulong id)
        {
            ClientProfile profile = _profiles.Find(x => x.Id == id);
            
            if (profile == null)
                throw new Exception("Profile not found for id: " + id);
                
            _profiles.Remove(profile);
            OnClientRemoved?.Invoke(id);
        }

        public ClientProfile GetById(ulong clientId)
        {
            ClientProfile foundProfile = _profiles.Find(x => x.Id == clientId);
            return foundProfile;
        }
        
        public ClientProfile GetFirstByRole(GameRole role)
        {
            ClientProfile foundProfile = _profiles.FirstOrDefault(x => x.Role == role);
            return foundProfile;
        }

        public void Clear() => 
            _profiles.Clear();
    }
}