using System;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Code.Runtime.Utils
{
    public class LookAtCamera : MonoBehaviour
    {
        public UnityEngine.Camera _camera;
        
        private void Start() => 
            _camera = UnityEngine.Camera.main;

        private void LateUpdate()
        {
            var direction = transform.position - _camera.transform.position;
            quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = lookRotation;
        }
    }
}