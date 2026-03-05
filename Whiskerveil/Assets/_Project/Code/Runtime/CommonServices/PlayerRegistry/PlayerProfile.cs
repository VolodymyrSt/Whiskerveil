using _Project.Code.Runtime.CommonServices.RolePicker;

namespace _Project.Code.Runtime.CommonServices.PlayerRegistry
{
    public class PlayerProfile
    {
        public ulong Id;
        public GameRole Role;
        public string SlotId = string.Empty;

        public PlayerProfile(ulong id) => 
            Id = id;

        public PlayerProfile WithRole(GameRole role)
        {
            Role = role;
            return this;
        }
    }
}