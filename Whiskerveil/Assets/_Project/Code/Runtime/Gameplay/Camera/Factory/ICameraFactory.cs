using UnityEngine;

namespace _Project.Code.Runtime.Gameplay.Camera.Factory
{
    public interface ICameraFactory
    {
        IFPVCameraHandler CreateCamera(Transform under);
    }
}