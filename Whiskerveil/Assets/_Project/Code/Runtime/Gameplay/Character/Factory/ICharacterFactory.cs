using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.Gameplay.Character.Preview;
using UnityEngine;

namespace _Project.Code.Runtime.Gameplay.Character.Factory
{
    public interface ICharacterFactory
    {
        ICharacter CreateCharacter(ulong clientId,string clientName, GameRole @as , Vector3 at = default, Quaternion atRot = default);
        ICharacter CreateCharacterByProfile(ClientProfile profile, Vector3 at, Quaternion atRot);
    }
}