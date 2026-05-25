using System;
using _Project.Code.Runtime.CommonServices.SceneLoader;
using _Project.Code.Runtime.CommonServices.StaticData;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Infrustructure.EntryPoints
{
    public class GameBootstrapper : MonoBehaviour
    {
        private ISceneLoader _sceneLoader;
        private LoadingCurtain _loadingCurtain;
        private IStaticDataService _staticDataService;
        
        [Inject]
        private void Construct(ISceneLoader sceneLoader, LoadingCurtain  loadingCurtain
            , IStaticDataService staticDataService)
        {
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
            _staticDataService = staticDataService;
        }

        private async void Awake() => await RunGame();

        private async UniTask RunGame()
        {
            // _loadingCurtain.Procced();

            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
            _staticDataService.Initialize();
            _loadingCurtain.gameObject.SetActive(false);
            _sceneLoader.Load(SceneList.Menu);
            
            DontDestroyOnLoad(gameObject);
        }
    }
}