using System;

namespace _Project.Code.Runtime.CommonServices.GameState
{
    [Serializable]
    public class ClientLobbyState
    {
        public ulong ClientId;
        public bool IsReadyToPlay;
    }
}