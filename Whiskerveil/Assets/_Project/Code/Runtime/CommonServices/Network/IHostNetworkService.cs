using System;

namespace _Project.Code.Runtime.CommonServices.Network
{
    public interface IHostNetworkService
    {
        void StartHost();
        event Action<ulong> OnClientJoinedLobby;
        event Action<ulong> OnHostJoinedLobby;
    }
}