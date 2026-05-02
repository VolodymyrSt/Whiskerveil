using System;
using _Project.Code.Runtime.Infrustructure.AssetsManagement;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Gameplay.Camera.Factory
{
    public class CameraFactory : ICameraFactory
    {
        private readonly IAssetsProvider _assetsProvider;
        private readonly IInstantiator _instantiator;

        public CameraFactory(IAssetsProvider assetsProvider, IInstantiator instantiator)
        {
            _assetsProvider = assetsProvider;
            _instantiator = instantiator;
        }

        public IFPVCameraHandler CreateCamera(Transform under)
        {
            FPVCameraHandler prefab = _assetsProvider.Load<FPVCameraHandler>(AssetsPath.FPVCameraPath);
            FPVCameraHandler instance = _instantiator.InstantiatePrefabForComponent<FPVCameraHandler>(prefab, under);
            return instance;
        }
    }
}