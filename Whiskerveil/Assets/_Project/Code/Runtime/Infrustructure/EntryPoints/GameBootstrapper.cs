using System;
using _Project.Code.Runtime.CommonServices.SceneLoader;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Infrustructure.EntryPoints
{
    public class GameBootstrapper : MonoBehaviour
    {
        private ISceneLoader _sceneLoader;
        private LoadingCurtain _loadingCurtain;
        
        [Inject]
        private void Construct(ISceneLoader sceneLoader, LoadingCurtain  loadingCurtain)
        {
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
        }

        private void Awake() => RunGame();

        private void RunGame()
        {
            // _loadingCurtain.Procced();
            _loadingCurtain.gameObject.SetActive(false);
            _sceneLoader.Load(SceneList.Menu);
            
            DontDestroyOnLoad(gameObject);
        }
    }
}