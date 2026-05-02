using _Project.Code.Runtime.CommonServices.WindowManagement;
using _Project.Code.Runtime.Gameplay.UI.Windows;

namespace _Project.Code.Runtime.Gameplay.Factory.Window
{
    public interface IWindowsFactory
    {
        BaseWindow CreateWindow(WindowId windowId);
    }
}