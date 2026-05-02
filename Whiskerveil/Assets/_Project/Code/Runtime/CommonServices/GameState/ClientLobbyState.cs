using System;
using _Project.Code.Runtime.Gameplay.Character.Preview;

namespace _Project.Code.Runtime.CommonServices.GameState
{
    [Serializable]
    public class ClientLobbyState
    {
        public ulong ClientId;
        public bool IsReadyToPlay;
        public IPreview Preview;
    }
}