using _Project.Code.Runtime.CommonServices.InputManagement;
using _Project.Code.Runtime.CommonServices.StaticData;
using _Project.Code.Runtime.Gameplay.Character;
using UnityEngine;

namespace _Project.Code.Runtime.Gameplay.Camera
{
    public interface IFPVCameraHandler
    {
        void Init(ICharacter character, IInputService inputService, IStaticDataService staticDataService);
        void WithModifiers(bool withModifiers);
        void BlockCharacterLook();
        void ReleaseCharacterLook();
        void LateTick();
        Quaternion Rotation { get; }
    }
}