using System;
using System.Collections.Generic;
using _Project.Code.Runtime.Character;

namespace _Project.Code.Runtime.CommonServices.NetworkServices
{
    public interface IHostNetworkService
    {
        void StartHost();
        event Action<ulong, ICharacter> OnClientJoinedLobby;
        event Action<ulong, ICharacter> OnHostJoinedLobby;
    }
}