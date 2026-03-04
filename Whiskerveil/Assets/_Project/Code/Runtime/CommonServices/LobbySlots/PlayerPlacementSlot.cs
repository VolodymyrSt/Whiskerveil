using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Code.Runtime.CommonServices.LobbySlots
{
    public class PlayerPlacementSlot : MonoBehaviour
    {
        [SerializeField] private GameRole _forRole;
        [SerializeField] private string _id;
        
        public GameRole ForRole => _forRole;
        public string Id => _id;
        
        [Button("GenerateNewId", ButtonSizes.Medium, ButtonStyle.Box)]
        private void GenerateNewId() => _id = Util.GetUniqueId();
    }
}
