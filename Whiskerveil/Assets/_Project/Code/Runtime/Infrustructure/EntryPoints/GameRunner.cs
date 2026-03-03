using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Infrustructure.EntryPoints
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField] private GameBootstrapper _gameBootstrapperPrefab;
        
        private IInstantiator _instantiator;
        
        [Inject]
        private void Construct(IInstantiator instantiator) => 
            _instantiator = instantiator;

        private void Awake()
        {
            var gameBootstrapper = FindObjectOfType<GameBootstrapper>();
            
            if (gameBootstrapper != null) return;

            _instantiator.InstantiatePrefabForComponent<GameBootstrapper>(_gameBootstrapperPrefab);
        }
    }
}