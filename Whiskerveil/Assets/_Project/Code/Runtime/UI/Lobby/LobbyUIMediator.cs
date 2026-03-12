using System.Collections.Generic;
using _Project.Code.Runtime.Character;
using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.GameState;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.SwapRole;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Code.Runtime.UI.Lobby
{
    public class LobbyUIMediator : NetworkBehaviour
    {
        [SerializeField] private Button _readyButton;
        [SerializeField] private Button _swapRoleButton;
        
        [SerializeField] private Image _readySigh;
        
        [SerializeField] private SwapRoleWindow _swapRoleWindow;
        
        private IClientsRegistry _clientsRegistry;
        private ISwapRoleService _swapRoleService;
        private IGameStateService _gameStateService;
        
        [Inject]
        public void Construct(IClientsRegistry clientsRegistry, ISwapRoleService swapRoleService,
            IGameStateService gameStateService)
        {
            _clientsRegistry = clientsRegistry;
            _swapRoleService = swapRoleService;
            _gameStateService = gameStateService;
        }
        
        public override void OnNetworkSpawn()
        {
            if (IsServer)
                _gameStateService.OnLobbyStateChanged += OnLobbyStateChanged;
            
            _readySigh.gameObject.SetActive(false);
            _swapRoleWindow.Hide();
            
            _readyButton.onClick.AddListener(() => OnReadyButtonPressedServerRpc());
            _swapRoleButton.onClick.AddListener(() => RequestSwapRoleServerRpc());
        }


        [ServerRpc(RequireOwnership = false)]
        private void OnReadyButtonPressedServerRpc(ServerRpcParams rpcParams = default)
        {
            ClientLobbyState updatedState = _gameStateService
                .UpdateClientLobbyState(rpcParams.Receive.SenderClientId);
            
            ToggleSwapRoleButtonFor(rpcParams.Receive.SenderClientId, updatedState.IsReadyToPlay);

            OnReadyButtonPressedClientRpc(updatedState.IsReadyToPlay, new ClientRpcParams {
                Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { rpcParams.Receive.SenderClientId } }
            });
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestSwapRoleServerRpc(ServerRpcParams rpcParams = default)
        {
            ulong senderId = rpcParams.Receive.SenderClientId;
            ClientProfile requesterProfile = _clientsRegistry.GetById(senderId);
            
            HideSwapRoleButtonClientRpc();
            
            foreach (ClientProfile profile in _clientsRegistry.Profiles)
            {
                if (profile.Id == senderId) continue;
                if (profile.Role == requesterProfile.Role) continue;

                var onlyToTarget = new ClientRpcParams {
                    Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { profile.Id } }
                };
                
                ShowSwapRoleWindowToClientRpc(requesterProfile.Name, requesterProfile.Role, senderId, onlyToTarget);
            }
        }
        
        [ClientRpc]
        private void ShowSwapRoleWindowToClientRpc(FixedString64Bytes requesterName, GameRole requesterRole,
            ulong requesterId, ClientRpcParams clientRpcParams = default)
        {
            _swapRoleWindow.Show(requesterName, requesterRole, () =>
                AcceptSwapRoleServerRpc(requesterId), _swapRoleWindow.Hide);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void AcceptSwapRoleServerRpc(ulong requesterId, ServerRpcParams rpcParams = default)
        {
            _swapRoleService.SwapRoleBetween(requesterId, rpcParams.Receive.SenderClientId);
            
            HideSwapRoleWindowClientRpc();
            ShowSwapRoleButtonClientRpc();
        }
        
        [ClientRpc]
        private void HideSwapRoleWindowClientRpc() => 
            _swapRoleWindow.Hide();

        [ClientRpc]
        private void HideSwapRoleButtonClientRpc(ClientRpcParams rpcParams = default) => 
            _swapRoleButton.gameObject.SetActive(false);
        
        [ClientRpc]
        private void ShowSwapRoleButtonClientRpc(ClientRpcParams rpcParams = default) => 
            _swapRoleButton.gameObject.SetActive(true);
        
        [ClientRpc]
        private void ShowReadyButtonClientRpc(ClientRpcParams rpcParams = default) => 
            _readyButton.gameObject.SetActive(true);
        
        [ClientRpc]
        private void HideReadyButtonClientRpc() => 
            _readyButton.gameObject.SetActive(false);
        
        [ClientRpc]
        private void OnReadyButtonPressedClientRpc(bool condition, ClientRpcParams rpcParams = default) => 
            _readySigh.gameObject.SetActive(condition);

        private void ToggleSwapRoleButtonFor(ulong clientId, bool condition)
        {
            if (condition) {
                HideSwapRoleButtonClientRpc(new ClientRpcParams() {
                    Send = new ClientRpcSendParams { TargetClientIds = new ulong[] {clientId} }
                });
            }
            else {
                ShowSwapRoleButtonClientRpc(new ClientRpcParams() {
                    Send = new ClientRpcSendParams { TargetClientIds = new ulong[] {clientId} }
                });
            }
        }

        private void OnLobbyStateChanged(int actualClientStatesCount)
        {
            if (actualClientStatesCount < 2)
                HideReadyButtonClientRpc();
            else
                ShowReadyButtonClientRpc();
            
            if (actualClientStatesCount >= 2)
                ShowSwapRoleButtonClientRpc();
            else
                HideSwapRoleButtonClientRpc();
        }
        
        public override void OnNetworkDespawn()
        {
            if (IsServer)
                _gameStateService.OnLobbyStateChanged -= OnLobbyStateChanged;
            
            _readyButton.onClick.RemoveAllListeners();
            _swapRoleButton.onClick.RemoveAllListeners();
        }
    }
}