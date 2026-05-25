using _Project.Code.Runtime.CommonServices.SceneLoader;
using _Project.Code.Runtime.CommonServices.WindowManagement;
using Zenject;

namespace _Project.Code.Runtime.Gameplay.UI.Windows
{
    public class RatVictoryWindow : BaseWindow
    {
        private ISceneLoader _sceneLoader;
        
        [Inject]
        private void Construct(ISceneLoader sceneLoader) => 
            _sceneLoader = sceneLoader;
        
        protected override void Initialize() => 
            Id = WindowId.RatVictory;
    }
}