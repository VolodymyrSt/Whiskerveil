using System;

namespace _Project.Code.Runtime.CommonServices.WindowManagement
{
    public interface IWindowService
    {
        void Open(WindowId windowId, Action whenClosed = null);
        void Close(WindowId windowId);
    }
}