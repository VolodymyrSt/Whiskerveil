using System;

namespace _Project.Code.Runtime.CommonServices.SceneLoaderService
{
    public interface ISceneLoader
    {
        void Load(string sceneName, Action onLoaded = null);
        void LoadSync(string sceneName, Action onLoaded = null);
    }
}