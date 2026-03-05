using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Infrustructure.AssetsManagement
{
    public class AssetsProvider : IAssetsProvider
    {
        private readonly DiContainer _container;

        public AssetsProvider(DiContainer container) => 
            _container = container;

        public T Instantiate<T>(string path) where T : Object
        {
            var prefab = Load<T>(path);
            var instance = Object.Instantiate(prefab);
            _container.Inject(instance);
            return instance;
        }   
        
        public T Instantiate<T>(string path, Vector3 at) where T : Object
        {
            var prefab = Load<T>(path);
            var instance = NetworkObject.Instantiate(prefab, at, Quaternion.identity);
            _container.Inject(instance);
            return instance;
        }   
        
        public T Instantiate<T>(string path, Vector3 at, Quaternion atRot) where T : Object
        {
            var prefab = Load<T>(path);
            var instance = NetworkObject.Instantiate(prefab, at, atRot);
            _container.Inject(instance);
            return instance;
        } 
        
        public T Instantiate<T>(string path, Vector3 at, Transform parent) where T : Object
        {
            var prefab = Load<T>(path);
            var instance = Object.Instantiate(prefab, at, Quaternion.identity, parent);
            _container.Inject(instance);
            return instance;
        }
        
        public T Instantiate<T>(string path, Transform parent) where T : Object
        {
            var prefab = Load<T>(path);
            var instance = Object.Instantiate(prefab, parent);
            _container.Inject(instance);
            return instance;
        }

        public GameObject Instantiate(string path)
        {
            var prefab = Load<GameObject>(path);
            var instance = Object.Instantiate(prefab);
            _container.Inject(instance);
            return instance;
        }
        
        public GameObject Instantiate(string path, Transform parent)
        {
            var prefab = Load<GameObject>(path);
            var instance = Object.Instantiate(prefab, parent);
            _container.Inject(instance);
            return instance;
        }

        public GameObject Instantiate(string path, Vector3 at, Transform parent)
        {
            var prefab = Load<GameObject>(path);
            var instance = Object.Instantiate(prefab, at, Quaternion.identity, parent);
            _container.Inject(instance);
            return instance;
        }
        
        public T Load<T>(string path) where T : Object =>
            Resources.Load<T>(path);
    }
}