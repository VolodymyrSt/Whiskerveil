using System;
using System.Collections.Generic;
using _Project.Code.Runtime.CommonServices.RolePicker;

namespace _Project.Code.Runtime.CommonServices.ClientRegistry
{
    public interface IClientsRegistry
    {
        void AddProfile(ClientProfile clientProfile);
        ClientProfile GetById(ulong client);
        void Clear();
        void RemoveProfile(ulong id);
        List<ClientProfile> Profiles { get; }
        bool IsEmpty { get; }
        int TotalCount { get; }
        event Action<ClientProfile> OnNewClientAdded;
        event Action<ulong> OnClientRemoved;
        ClientProfile GetFirstByRole(GameRole role);
    }
}