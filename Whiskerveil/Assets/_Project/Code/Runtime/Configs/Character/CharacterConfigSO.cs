using _Project.Code.Runtime.CommonServices.RolePicker;
using UnityEngine;

namespace _Project.Code.Runtime.Configs.Character
{
    [CreateAssetMenu(fileName = "Character Config", menuName = "Configs/Character")]
    public class CharacterConfigSO : ScriptableObject
    {
        [Header("Role")]
        public GameRole ForRole;
        
        [Header("Speed")]
        public float WalkSpeed;
        public float SprintSpeed = 9;
        public float AirSpeed = 15f;
        
        [Header("Response")]
        public float WalkResponse = 20f;
        public float SprintResponse = 30f;
        
        [Header("Jump")]
        public float JumpForce;
        public float CoyoteTime = 0.12f;
        
        [Header("Collider")]
        public float StandHeight = 2f;
        public float ColliderRadius = 2f;
        public float YOffset = 2f;
        
        [Header("CameraStanceHeight")]
        public float StandCameraTargetHeight = 0.9f;
        
        [Header("Air")]
        public float AirAcceleration = 70f;
    }
}