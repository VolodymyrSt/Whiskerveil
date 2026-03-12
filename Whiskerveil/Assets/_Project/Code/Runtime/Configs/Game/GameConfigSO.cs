using UnityEngine;

namespace _Project.Code.Runtime.Configs.Game
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/Game/GameConfig")]
    public class GameConfigSO : ScriptableObject
    {
        [Header("Sprites")]
        public Sprite RatSprite;
        public Sprite CatSprite;
    }
}