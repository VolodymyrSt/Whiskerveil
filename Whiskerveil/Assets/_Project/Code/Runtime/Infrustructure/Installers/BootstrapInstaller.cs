using _Project.Code.Runtime.Character.Factory;
using _Project.Code.Runtime.CommonServices.NetworkServices;
using _Project.Code.Runtime.CommonServices.RolePickerService;
using _Project.Code.Runtime.CommonServices.SceneLoaderService;
using _Project.Code.Runtime.Infrustructure.AssetsManagement;
using _Project.Code.Runtime.Utils;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Infrustructure.Installers
{
    public class BootstrapInstaller : MonoInstaller, ICoroutineRunner
    {
        [SerializeField] private LoadingCurtain _loadingCurtain;
        
        public override void InstallBindings()
        {
            Container.Bind<LoadingCurtain>().FromComponentInNewPrefab(_loadingCurtain).AsSingle().NonLazy();
            
            BindNetworkServices();
            BindAssetsProvider();

            BindCoroutineRunner();
            BindSceneLoader();
            BindRolePicker();
            
            BindCharacterFactory();
        }

        private void BindAssetsProvider()
        {
            Container.BindInterfacesTo<AssetsProvider>().AsSingle();
        }

        private void BindNetworkServices()
        {
            Container.BindInterfacesTo<HostNetworkService>().AsSingle();
            Container.BindInterfacesTo<ClientNetworkService>().AsSingle();
        }

        private void BindCharacterFactory() => 
            Container.BindInterfacesTo<CharacterFactory>().AsSingle();

        private void BindRolePicker() => 
            Container.Bind<IRolePicker>().To<RolePicker>().AsSingle();

        private void BindSceneLoader() => 
            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();

        private void BindCoroutineRunner() => 
            Container.Bind<ICoroutineRunner>().FromInstance(this).AsSingle();
    }
}