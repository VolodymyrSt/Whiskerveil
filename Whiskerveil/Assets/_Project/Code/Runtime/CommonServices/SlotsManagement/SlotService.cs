using System.Collections.Generic;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.StaticData;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Code.Runtime.CommonServices.SlotsManagement
{
    public class SlotService : ISlotService
    {
        private readonly List<LobbySlot> _lobbySlots = new();
        private readonly List<BaseSlot> _levelSlots = new();
        
        private readonly IStaticDataService _staticDataService;
        
        public SlotService(IStaticDataService staticDataService) => 
            _staticDataService = staticDataService;

        public void Initialize()
        {
            if (!NetworkManager.Singleton.IsServer) return;
            
            _lobbySlots.Clear();
            _levelSlots.Clear();

            foreach (var slotData in _staticDataService.LobbySlotsDataHolder.Slots)
                _lobbySlots.Add(new LobbySlot(slotData.Id, slotData.ForRole, slotData.Position, slotData.Rotation));
            
            foreach (var slotData in _staticDataService.LevelSlotsDataHolder.Slots)
                _levelSlots.Add(new BaseSlot(slotData.ForRole, slotData.Position, slotData.Rotation));
        }

        public void DebugAllLobbySlots()
        {
            foreach (var slotData in _lobbySlots)
                Debug.Log($"[SlotService] Adding lobby slot: id={slotData.Id}, role={slotData.ForRole}");
        }

        public void PrepareLobbySlots()
        {
            if (!NetworkManager.Singleton.IsServer) return;
            
            foreach (var slot in _lobbySlots)
                slot.IsTaken = false;
        }

        public void PrepareLevelSlots()
        {
            if (!NetworkManager.Singleton.IsServer) return;
            
            foreach (var slot in _levelSlots)
                slot.IsTaken = false;
        }
        
        public BaseSlot GetFreeLevelSlotFor(GameRole role)
        {
            BaseSlot foundSlot = _levelSlots.Find(x => !x.IsTaken && x.ForRole == role);

            if (foundSlot != null)
                return foundSlot;
            
            throw new KeyNotFoundException($"LevelSlot with role {role} not available");
        }
        
        public LobbySlot GetFreeLobbySlotFor(GameRole role)
        {
            LobbySlot foundSlot = _lobbySlots.Find(x => !x.IsTaken && x.ForRole == role);

            if (foundSlot != null)
                return foundSlot;
            
            throw new KeyNotFoundException($"LobbySlot with role {role} not available");
        }

        public LobbySlot GetLobbySlotById(FixedString64Bytes id)
        {
            LobbySlot foundSlot = _lobbySlots.Find(x => x != null && x.Id == id);

            if (foundSlot != null)
                return foundSlot;
            
            throw new KeyNotFoundException($"LobbySlot with id {id} not found");
        }
    }
}