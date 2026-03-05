using UnityEngine;

namespace _Project.Code.Runtime.Infrustructure.AssetsManagement
{
    public interface IAssetsProvider
    {
        T Instantiate<T>(string path) where T : Object;
        T Instantiate<T>(string path, Vector3 at, Transform parent) where T : Object;
        GameObject Instantiate(string path);
        GameObject Instantiate(string path, Vector3 at, Transform parent);
        T Load<T>(string path) where T : Object;
        T Instantiate<T>(string path, Vector3 at) where T : Object;
        T Instantiate<T>(string path, Vector3 at, Quaternion atRot) where T : Object;
    }
}