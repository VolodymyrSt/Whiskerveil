using _Project.Code.Runtime.Character.Factory;
using _Project.Code.Runtime.CommonServices.LobbySlots;
using _Project.Code.Runtime.CommonServices.Network;
using _Project.Code.Runtime.CommonServices.PlayerRegistry;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.SceneLoader;
using _Project.Code.Runtime.Configs.Lobby;
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
        [SerializeField] private LobbySlotsDataHolder lobbySlotsDataHolder;
        
        public override void InstallBindings()
        {
            BindLoadingCurtain();
            
            BindNetworkServices();
            BindAssetsProvider();

            BindCoroutineRunner();
            BindSceneLoader();
            BindRolePicker();
            
            BindCharacterFactory();
            
            Container.BindInterfacesTo<LobbySlotService>().AsSingle().WithArguments(lobbySlotsDataHolder);
            
            Container.BindInterfacesTo<PlayersRegistry>().AsSingle();
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