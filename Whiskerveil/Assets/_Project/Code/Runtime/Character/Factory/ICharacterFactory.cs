using _Project.Code.Runtime.Character.View;
using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.RolePicker;
using UnityEngine;

namespace _Project.Code.Runtime.Character.Factory
{
    public interface ICharacterFactory
    {
        ICharacter CreateCharacter(ulong clientId,string clientName, GameRole @as , Vector3 at = default, Quaternion atRot = default);
        ICharacter CreateCharacterByProfile(ClientProfile profile, Vector3 at, Quaternion atRot);
    }
}