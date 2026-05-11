using System.Collections;
using _Project.Code.Runtime.CommonServices.SceneLoader;
using _Project.Code.Runtime.CommonServices.WindowManagement;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Code.Runtime.Gameplay.UI.Windows
{
    public class CatVictoryWindow : BaseWindow
    {
        private ISceneLoader _sceneLoader;
        
        [Inject]
        private void Construct(ISceneLoader sceneLoader) => 
            _sceneLoader = sceneLoader;
        
        protected override void Initialize() => 
            Id = WindowId.CatVictory;
    }
}