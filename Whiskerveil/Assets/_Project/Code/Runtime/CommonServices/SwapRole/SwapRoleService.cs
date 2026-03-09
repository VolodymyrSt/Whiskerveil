using System;
using _Project.Code.Runtime.Character;
using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.LobbySlots;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.Utils;
using Unity.Android.Gradle.Manifest;
using Unity.Collections;
using Unity.Netcode;

namespace _Project.Code.Runtime.CommonServices.SwapRole
{
    public class SwapRoleService : ISwapRoleService
    {
        private readonly IClientsRegistry _clientsRegistry;
        private readonly ILobbySlotService _lobbySlotService;

        public SwapRoleService(IClientsRegistry clientsRegistry, ILobbySlotService lobbySlotService)
        {
            _clientsRegistry = clientsRegistry;
            _lobbySlotService = lobbySlotService;
        }
        
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
            
            var fromSlot = _lobbySlotService.GetSlotById(fromProfile.SlotId);
            var toSlot = _lobbySlotService.GetSlotById(toProfile.SlotId);
            
            fromCharacter.Transform.SetPositionAndRotation(toSlot.Position, toSlot.Rotation);
            toCharacter.Transform.SetPositionAndRotation(fromSlot.Position, fromSlot.Rotation);
            
            fromProfile.SwapData(toProfile);
        }
    }
}