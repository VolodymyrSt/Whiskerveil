using System;
using System.Collections.Generic;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.SlotsManagement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Code.Runtime.Configs.Slots
{
    [CreateAssetMenu(fileName = "SlotsHolder", menuName = "Configs/Slots/SlotsHolder")]
    public class SlotsDataHolderSO : ScriptableObject
    {
        public List<SlotData> Slots = new();

        [Button("Find slots", ButtonSizes.Medium, ButtonStyle.Box)]
        private void Find()
        {
            Slots.Clear();
            
            PlayerPlacementSlot[] playerPlacementSlots = FindObjectsByType<PlayerPlacementSlot>(FindObjectsSortMode.InstanceID);

            foreach (PlayerPlacementSlot playerPlacementSlot in playerPlacementSlots)
            {
                Slots.Add(new SlotData {
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
    public struct SlotData
    {
        public string Name;
        public string Id;
        public GameRole ForRole;
        public Vector3 Position;
        public Quaternion Rotation;
    }
}