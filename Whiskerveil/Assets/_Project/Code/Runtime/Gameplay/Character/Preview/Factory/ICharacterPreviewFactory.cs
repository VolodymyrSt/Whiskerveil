using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.RolePicker;
using UnityEngine;

namespace _Project.Code.Runtime.Gameplay.Character.Preview.Factory
{
    public interface ICharacterPreviewFactory
    {
        IPreview CreatePreviewByProfile(ClientProfile profile, Vector3 at, Quaternion atRot);
        IPreview CreatePreview(ulong clientId, string clientName, GameRole @as, Vector3 at, Quaternion atRot);
    }
}