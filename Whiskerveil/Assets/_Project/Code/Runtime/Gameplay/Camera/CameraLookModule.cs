using _Project.Code.Runtime.Configs.Camera;
using UnityEngine;

namespace _Project.Code.Runtime.Gameplay.Camera
{
    public class CameraLookModule
    {
        private readonly Transform _cameraTransform; 
        private readonly CameraConfigSO _config; 
        
        private readonly float _sensitivity = 80f;  //Change to SaveLoadData
        private readonly float _lookResponse;
        
        private float _minPitch; 
        private float _maxPitch;
        
        private float _yaw;
        private float _pitch;
        
        private float _minYaw; 
        private float _maxYaw;
        
        private Vector2 _currentLook;
        
        private Transform _vehicle;
        private float _lastVehicleYaw;
        
        
        public CameraLookModule(Transform cameraTransform, CameraConfigSO config)
        {
            _cameraTransform = cameraTransform;
            _config = config;
            _lookResponse = config.LookResponse;
            
            InitializeYawPitch(cameraTransform);
        }

        public void UpdateLook(Vector3 inputLook) 
        {
            _currentLook = Vector2.Lerp(_currentLook, inputLook, 1f - Mathf.Exp(-_lookResponse * Time.deltaTime));
            
            _yaw += _currentLook.x * _sensitivity * Time.deltaTime;
            _pitch -= _currentLook.y * _sensitivity * Time.deltaTime;
            
            ApplyClamp();
            ApplyRotation();
        }

        private void ApplyRotation() => 
            _cameraTransform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);

        private void ApplyClamp() => 
            _pitch = Mathf.Clamp(_pitch, _config.MinPitch, _config.MaxPitch);

        private void InitializeYawPitch(Transform cameraTransform)
        {
            var euler = cameraTransform.rotation.eulerAngles;
            _yaw = euler.y;
            _pitch = euler.x > 180 ? euler.x - 360 : euler.x;
        }
    }
}