using _Project.Code.Runtime.Character.View;
using _Project.Code.Runtime.CommonServices.RolePickerService;
using UnityEngine;

namespace _Project.Code.Runtime.Character.Factory
{
    public interface ICharacterFactory
    {
        ICharacter CreateCharacter(ulong clientId, GameRole @as , Vector3 at = default);
    }
}