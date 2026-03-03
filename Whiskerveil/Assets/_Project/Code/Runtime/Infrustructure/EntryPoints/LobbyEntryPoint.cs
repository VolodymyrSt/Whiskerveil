using _Project.Code.Runtime.Character;
using _Project.Code.Runtime.CommonServices.LobbySlotServices;
using _Project.Code.Runtime.CommonServices.NetworkServices;
using _Project.Code.Runtime.CommonServices.RolePickerService;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Infrustructure.EntryPoints
{
    public class LobbyEntryPoint : NetworkBehaviour
    {
        private IHostNetworkService _hostNetworkService;
        private ILobbySlotService _lobbySlotService;

        [Inject]
        public void Construct(IHostNetworkService hostNetworkService, ILobbySlotService lobbySlotService)
        {
            _hostNetworkService = hostNetworkService;
            _lobbySlotService = lobbySlotService;
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            
            _hostNetworkService.OnClientJoinedLobby += OnClientJoinedLobby;
            _hostNetworkService.OnHostJoinedLobby += OnHostJoinedLobby;
        }

        private void OnHostJoinedLobby(ulong id, ICharacter hostPlayer) => 
            AssignPlayerToSlot(id, hostPlayer, GameRole.Seeker);

        private void OnClientJoinedLobby(ulong id, ICharacter clientPlayer) => 
            AssignPlayerToSlot(id, clientPlayer, GameRole.Hider);

        private void AssignPlayerToSlot(ulong playerId, ICharacter player, GameRole role)
        {
            if (!_lobbySlotService.TryGetFreeSlotFor(role, out var slot))
            {
                Debug.LogWarning($"No free slot for {role}!");
                return;
            }

            player.Transform.SetPositionAndRotation(slot.transform.position, slot.transform.rotation);
            slot.Occupy(playerId);
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            
            _hostNetworkService.OnClientJoinedLobby -= OnClientJoinedLobby;
            _hostNetworkService.OnHostJoinedLobby -= OnHostJoinedLobby;
        }
    }
}