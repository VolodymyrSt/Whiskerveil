using UnityEngine;

namespace _Project.Code.Runtime.Configs.Camera
{
    [CreateAssetMenu(fileName = "Camera Config", menuName = "Configs/Camera")]
    public class CameraConfigSO : ScriptableObject
    {
        [Header("Look")]
        [Range(-90, 0)] public float MinPitch = -80f; 
        [Range(0, 90)] public float MaxPitch = 80f;
        [Range(0, 90)] public float LookResponse = 15f;
        
        [Header("Lean")]
        public float AttackDamping = 0.5f;
        public float DecayDamping = 0.3f;
        public float LeanStrength = 0.075f;
        public float StrengthResponse = 5f;
        
        [Header("Spring")]
        public float HalfLife = 0.075f;
        public float Frequency = 18f;
        public float AngularDisplacement = 2f;
        public float LinerDisplacement = 0.05f;
    }
}