using System;
using System.Collections.Generic;
using _Project.Code.Runtime.Factory.Window;
using _Project.Code.Runtime.UI.Windows;
using UnityEditor.PackageManager.UI;
using Object = UnityEngine.Object;

namespace _Project.Code.Runtime.CommonServices.WindowManagement
{
    public class WindowService : IWindowService
    {
        private readonly Dictionary<WindowId, Action> _callbacksWhenClosed = new();
        private readonly IWindowsFactory _windowFactory;

        private readonly List<BaseWindow> _openedWindows = new();

        public WindowService(IWindowsFactory windowFactory) =>
            _windowFactory = windowFactory;

        public void Open(WindowId windowId, Action whenClosed = null)
        {
            _openedWindows.Add(_windowFactory.CreateWindow(windowId));
            
            if (whenClosed != null)
                _callbacksWhenClosed.Add(windowId, whenClosed);
        }

        public void Close(WindowId windowId)
        {
            BaseWindow window = _openedWindows.Find(x => x.Id == windowId);
            if (window == null) return;

            if (_callbacksWhenClosed.TryGetValue(windowId, out Action whenClosed))
            {
                whenClosed?.Invoke();
                _callbacksWhenClosed.Remove(windowId);
            }
            
            _openedWindows.Remove(window);
            Object.Destroy(window.gameObject);
        }
    }
}