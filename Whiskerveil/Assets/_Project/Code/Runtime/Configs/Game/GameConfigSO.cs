using UnityEngine;

namespace _Project.Code.Runtime.Configs.Game
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/Game/GameConfig")]
    public class GameConfigSO : ScriptableObject
    {
        [Header("Base")]
        public float HidingTime = 30f;
        public float SeekingTime = 300f;
        
        [Header("Sprites")]
        public Sprite RatSprite;
        public Sprite CatSprite;
    }
}