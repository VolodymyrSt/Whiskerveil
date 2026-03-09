using System;

namespace _Project.Code.Runtime.CommonServices.Network
{
    public interface IHostNetworkService
    {
        void StartHost(string nickname);
        event Action<ulong> OnClientDisconnected;
    }
}