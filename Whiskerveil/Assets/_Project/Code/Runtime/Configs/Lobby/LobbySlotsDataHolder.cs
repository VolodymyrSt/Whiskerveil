using System;
using System.Collections.Generic;
using _Project.Code.Runtime.CommonServices.LobbySlots;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Code.Runtime.Configs.Lobby
{
    [CreateAssetMenu(fileName = "LobbySlotsHolder", menuName = "Configs/Lobby/LobbySlotsHolder")]
    public class LobbySlotsDataHolder : ScriptableObject
    {
        public List<LobbySlotData> Slots = new();

        [Button("Find slots", ButtonSizes.Medium, ButtonStyle.Box)]
        private void Find()
        {
            Slots.Clear();
            
            PlayerPlacementSlot[] playerPlacementSlots = FindObjectsByType<PlayerPlacementSlot>(FindObjectsSortMode.InstanceID);

            foreach (PlayerPlacementSlot playerPlacementSlot in playerPlacementSlots)
            {
                Slots.Add(new LobbySlotData {
                    Name     = playerPlacementSlot.gameObject.name,
                    Id       = playerPlacementSlot.Id,
                    ForRole  =  playerPlacementSlot.ForRole,
                    Position = playerPlacementSlot.transform.position,
                    Rotation = playerPlacementSlot.transform.rotation
                });
            }
        }
    }

    [Serializable]
    public struct LobbySlotData
    {
        public string Name;
        public string Id;
        public GameRole ForRole;
        public Vector3 Position;
        public Quaternion Rotation;
    }
}