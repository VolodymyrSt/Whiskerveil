using _Project.Code.Runtime.CommonServices.RolePicker;
using Unity.Collections;
using Unity.Netcode;

namespace _Project.Code.Runtime.CommonServices.ClientRegistry
{
    public class ClientProfile : INetworkSerializable
    {
        public FixedString64Bytes Name;
        public ulong Id;
        public GameRole Role;
        public FixedString64Bytes SlotId = string.Empty;
        public ClientProfile(){}
        public void SwapData(ClientProfile other)
        {
            (Role, other.Role) = (other.Role, Role);
            (SlotId, other.SlotId) = (other.SlotId, SlotId);
        }
        
        public ClientProfile(ulong id) => 
            Id = id;

        public ClientProfile WithName(string name)
        {
            Name = name;
            return this;
        }
        
        public ClientProfile WithRole(GameRole role)
        {
            Role = role;
            return this;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id);
            serializer.SerializeValue(ref Role);
            serializer.SerializeValue(ref Name);
            serializer.SerializeValue(ref SlotId);
        }
    }
}