using System;
using System.Collections.Generic;
using _Project.Code.Runtime.Character;
using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.SlotsManagement;
using _Project.Code.Runtime.Utils;
using UnityEngine;

namespace _Project.Code.Runtime.CommonServices.SwapRole
{
    public class SwapRoleService : ISwapRoleService
    {
        private readonly IClientsRegistry _clientsRegistry;
        private readonly ISlotService _slotService;
        
        private readonly List<ulong> _roleSwapPendingApprovers = new();
        private ulong? _swapRoleRequester = null;
        
        public bool HasRequester => _swapRoleRequester.HasValue;
        public bool HasApprovers => _roleSwapPendingApprovers.Count > 0;

        public SwapRoleService(IClientsRegistry clientsRegistry, ISlotService slotService)
        {
            _clientsRegistry = clientsRegistry;
            _slotService = slotService;
        }

        public void AssignRequester(ulong requesterId)
        {
            Debug.Log("SwapRoleService.SetRequester id: " + requesterId);
            _swapRoleRequester = requesterId;
        }

        public void ClearRequest()
        {
            _swapRoleRequester = null;
            _roleSwapPendingApprovers.Clear();
        }

        public bool IsRequester(ulong requesterId) => 
            _swapRoleRequester == requesterId;
        
        public bool IsApprover(ulong approverId) => 
            _roleSwapPendingApprovers.Contains(approverId);


        public void AddApprover(ulong clientId) => 
            _roleSwapPendingApprovers.Add(clientId);

        public void RemoveApprover(ulong clientId) => 
            _roleSwapPendingApprovers.Remove(clientId);
        
        public void SwapRoleBetween(ulong fromClientId, ulong toClientId)
        {
            if (!Net.IsServer) return;
            
            ICharacter fromCharacter = Util.GetComponentOnPlayerPrefab<ICharacter>(fromClientId);
            ICharacter toCharacter = Util.GetComponentOnPlayerPrefab<ICharacter>(toClientId);

            ClientProfile fromProfile = _clientsRegistry.Profiles.Find(x => x.Id == fromClientId);
            ClientProfile toProfile = _clientsRegistry.Profiles.Find(x => x.Id == toClientId);
            
            GameRole fromRole = fromProfile.Role;
            GameRole toRole = toProfile.Role;
            
            fromCharacter.AssignRole(toRole);
            toCharacter.AssignRole(fromRole);
            
            var fromSlot = _slotService.GetLobbySlotById(fromProfile.SlotId);
            var toSlot = _slotService.GetLobbySlotById(toProfile.SlotId);
            
            fromCharacter.Transform.SetPositionAndRotation(toSlot.Position, toSlot.Rotation);
            toCharacter.Transform.SetPositionAndRotation(fromSlot.Position, fromSlot.Rotation);
            
            fromProfile.SwapData(toProfile);
        }
    }
}