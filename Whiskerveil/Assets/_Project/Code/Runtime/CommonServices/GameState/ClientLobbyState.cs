using System;
using _Project.Code.Runtime.CommonServices.RolePicker;
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
    
    [Serializable]
    public class ClientGameplayState
    {
        public ulong ClientId;
        public bool IsDead;
        public GameRole Role;
    }
}