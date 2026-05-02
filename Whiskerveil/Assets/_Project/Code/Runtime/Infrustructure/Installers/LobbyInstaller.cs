using _Project.Code.Runtime.CommonServices.InputManagement;
using _Project.Code.Runtime.CommonServices.WindowManagement;
using _Project.Code.Runtime.Gameplay.Character.Factory;
using _Project.Code.Runtime.Gameplay.Character.Preview.Factory;
using _Project.Code.Runtime.Gameplay.Factory.Window;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Infrustructure.Installers
{
    public class LobbyInstaller : MonoInstaller
    {
        [SerializeField] private RectTransform _hubRoot;
        
        public override void InstallBindings()
        {
            BindWindowsFactory();
            BindWindowService();

            BindCharacterPreviewFactory();
        }
        
        private void BindCharacterPreviewFactory() => 
            Container.BindInterfacesTo<CharacterPreviewFactory>().AsSingle();

        private void BindWindowsFactory() =>
            Container.BindInterfacesTo<WindowsFactory>().AsSingle().WithArguments(_hubRoot);

        private void BindWindowService() =>
            Container.BindInterfacesTo<WindowService>().AsSingle();
    }
}