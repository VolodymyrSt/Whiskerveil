using _Project.Code.Runtime.CommonServices.InputManagement;
using _Project.Code.Runtime.CommonServices.StaticData;
using _Project.Code.Runtime.Gameplay.Character;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Gameplay.Camera
{
    public class FPVCameraHandler : MonoBehaviour, IFPVCameraHandler
    {
        [SerializeField] private CinemachineCamera _camera;
        
        private IInputService _input;
        private IStaticDataService _staticDataService;
        
        private Transform _target;
        public Quaternion Rotation => transform.rotation;
        
        private CameraLookModule _cameraLookModule;
        
        private bool _isInitialized = false;
        private bool _isLookBlocked = false;
        private bool _withModifiers = true;

        public void Init(ICharacter character, IInputService inputService, IStaticDataService staticDataService)
        {
            _input = inputService;
            _staticDataService = staticDataService;
            _target = character.Head;
            
            _cameraLookModule = new CameraLookModule(transform, _staticDataService.CameraConfig);
            
            transform.SetParent(character.CameraHolder, false);
            
            _isInitialized = true;
            _withModifiers = true;
            _isLookBlocked = false;
        }
        
        public void WithModifiers(bool withModifiers) => 
            _withModifiers = withModifiers;
        
        public void BlockCharacterLook()
        {
            if (_isLookBlocked) return;
            _isLookBlocked = true;
        }

        public void ReleaseCharacterLook()
        {
            if (!_isLookBlocked) return;
            _isLookBlocked = false;
        }

        public void LateTick()
        {
            if (!_isInitialized) return;
            
            UpdatePosition();
            UpdateLook();
            UpdateModifier();
        }

        private void UpdateModifier()
        {
            if (!_withModifiers) return;
            //_cameraSpringModifier.UpdateSpring(_target.up);
            //_cameraLeanModifier.UpdateLean(_character.Acceleration, _target.up);
        }

        private void UpdateLook()
        {
            if (!_isLookBlocked)
                _cameraLookModule.UpdateLook(_input.GetCharacterLookVector());
            else
                _cameraLookModule.UpdateLook(Vector3.zero);
        }

       private void UpdatePosition() =>
            _camera.transform.position = _target.position;
    }
}