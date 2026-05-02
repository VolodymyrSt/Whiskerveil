using _Project.Code.Runtime.CommonServices.WindowManagement;
using _Project.Code.Runtime.Gameplay.Camera.Factory;
using _Project.Code.Runtime.Gameplay.Character.Factory;
using _Project.Code.Runtime.Gameplay.Factory.Window;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Infrustructure.Installers
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private RectTransform _hubRoot;
        
        public override void InstallBindings()
        {
            BindWindowsFactory();
            BindWindowService();

            BindCharacterFactory();
            //BindCameraFactory();
        }
        
        private void BindCharacterFactory() => 
            Container.BindInterfacesTo<CharacterFactory>().AsSingle();

        private void BindCameraFactory() => 
            Container.BindInterfacesTo<CameraFactory>().AsSingle();

        private void BindWindowsFactory() =>
            Container.BindInterfacesTo<WindowsFactory>().AsSingle().WithArguments(_hubRoot);

        private void BindWindowService() =>
            Container.BindInterfacesTo<WindowService>().AsSingle();
    }
}