using _Project.Code.Runtime.CommonServices.WindowManagement;
using _Project.Code.Runtime.UI.Windows;

namespace _Project.Code.Runtime.Factory.Window
{
    public interface IWindowsFactory
    {
        BaseWindow CreateWindow(WindowId windowId);
    }
}