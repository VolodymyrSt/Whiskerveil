using _Project.Code.Runtime.Character.Factory;
using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.GameState;
using _Project.Code.Runtime.CommonServices.Network;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.SceneLoader;
using _Project.Code.Runtime.CommonServices.SlotsManagement;
using _Project.Code.Runtime.CommonServices.StaticData;
using _Project.Code.Runtime.CommonServices.SwapRole;
using _Project.Code.Runtime.Configs.Game;
using _Project.Code.Runtime.Factory.Window;
using _Project.Code.Runtime.Infrustructure.AssetsManagement;
using _Project.Code.Runtime.Infrustructure.EntryPoints;
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
            BindLoadingCurtain();
            
            BindNetworkServices();
            BindAssetsProvider();

            BindCoroutineRunner();
            BindSceneLoader();
            BindRolePicker();
            
            BindCharacterFactory();
            
            Container.BindInterfacesTo<SlotService>().AsSingle();
            Container.BindInterfacesTo<ClientsRegistry>().AsSingle();
            Container.BindInterfacesTo<SwapRoleService>().AsSingle();
            Container.BindInterfacesTo<GameStateService>().AsSingle();
            Container.BindInterfacesTo<StaticDataService>().AsSingle();
        }

        private void BindLoadingCurtain() => 
            Container.Bind<LoadingCurtain>().FromComponentInNewPrefab(_loadingCurtain).AsSingle().NonLazy();

        private void BindAssetsProvider() => 
            Container.BindInterfacesTo<AssetsProvider>().AsSingle();

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