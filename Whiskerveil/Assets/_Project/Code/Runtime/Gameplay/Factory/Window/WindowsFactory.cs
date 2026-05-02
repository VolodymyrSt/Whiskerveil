using _Project.Code.Runtime.CommonServices.StaticData;
using _Project.Code.Runtime.CommonServices.WindowManagement;
using _Project.Code.Runtime.Gameplay.UI.Windows;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Gameplay.Factory.Window
{
    public class WindowsFactory : IWindowsFactory
    {
        private readonly IInstantiator _instantiator;
        private readonly IStaticDataService _staticDataService;
        private readonly RectTransform _uiRoot;

        public WindowsFactory(IInstantiator instantiator, IStaticDataService staticDataService,
            RectTransform uiRoot)
        {
            _instantiator = instantiator;
            _staticDataService = staticDataService;
            _uiRoot = uiRoot;
        }
        
        public BaseWindow CreateWindow(WindowId windowId) =>
            _instantiator.InstantiatePrefabForComponent<BaseWindow>(PrefabFor(windowId), _uiRoot);
        
        private GameObject PrefabFor(WindowId id) =>
            _staticDataService.GetWindowPrefab(id);
    }
}