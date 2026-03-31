using System;
using _Project.Code.Runtime.UI.Windows;

namespace _Project.Code.Runtime.CommonServices.WindowManagement
{
    public interface IWindowService
    {
        void Open(WindowId windowId, Action whenClosed = null);
        void Close(WindowId windowId);
    }
}