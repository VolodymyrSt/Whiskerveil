using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Code.Runtime.Utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Code.Runtime.CommonServices.SceneLoader
{
    public class SceneLoader : ISceneLoader
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public SceneLoader(ICoroutineRunner coroutineRunner) => 
            _coroutineRunner = coroutineRunner;

        public void Load(string sceneName, Action onLoaded = null) => 
            _coroutineRunner.StartCoroutine(LoadScene(sceneName, onLoaded));   
        
        public void LoadSync(string sceneName, Action onLoaded = null) => 
            _coroutineRunner.StartCoroutine(LoadSceneSync(sceneName, onLoaded));

        private IEnumerator LoadScene(string sceneName, Action onLoaded = null)
        {
            if (SceneManager.GetActiveScene().name == sceneName)
            {
                onLoaded?.Invoke();
                yield break;
            }

            AsyncOperation wait = SceneManager.LoadSceneAsync(sceneName);

            while (!wait.isDone)
                yield return wait;

            onLoaded?.Invoke();
        }
        
        private IEnumerator LoadSceneSync(string sceneName, Action onLoaded = null)
        {
            if (SceneManager.GetActiveScene().name == sceneName)
            {
                onLoaded?.Invoke();
                yield break;
            }
            
            bool isLoaded = false;

            void SceneManagerOnOnLoadEventCompleted(string name, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
            {
                if (name == sceneName)
                {
                    isLoaded = true;
                    NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManagerOnOnLoadEventCompleted;
                }
            }

            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
            
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            
            while (!isLoaded)
                yield return null;

            onLoaded?.Invoke();
        }
    }
}