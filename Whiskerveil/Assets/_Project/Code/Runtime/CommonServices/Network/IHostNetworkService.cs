using System;
using Cysharp.Threading.Tasks;

namespace _Project.Code.Runtime.CommonServices.Network
{
    public interface IHostNetworkService
    {
        UniTask StartHost(string nickname);
        event Action<ulong> OnClientDisconnected;
        event Action<ulong> OnClientConnected;
        string JoinCode { get; }
    }
}