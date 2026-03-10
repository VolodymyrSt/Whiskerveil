using _Project.Code.Runtime.Character;
using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.GameState;
using _Project.Code.Runtime.CommonServices.LobbySlots;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.SceneLoader;
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
        [SerializeField] private Button _acceptSwapRoleButton;
        
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
            
            _acceptSwapRoleButton.gameObject.SetActive(false);
            
            _readyButton.onClick.AddListener(() => OnReadyButtonPressedServerRpc());
            _swapRoleButton.onClick.AddListener(() => RequestSwapRoleServerRpc());
        }


        [ServerRpc(RequireOwnership = false)]
        private void OnReadyButtonPressedServerRpc(ServerRpcParams rpcParams = default) => 
            _gameStateService.UpdateClientState(rpcParams.Receive.SenderClientId, true);

        [ServerRpc(RequireOwnership = false)]
        private void RequestSwapRoleServerRpc(ServerRpcParams rpcParams = default)
        {
            ulong senderId = rpcParams.Receive.SenderClientId;
            ClientProfile clientProfile = _clientsRegistry.GetById(senderId);
            
            HideSwapRoleButtonClientRpc();
            
            foreach (ClientProfile profile in _clientsRegistry.Profiles)
            {
                if (profile.Id == senderId) continue;
                if (profile.Role == clientProfile.Role) continue;

                var onlyToTarget = new ClientRpcParams {
                    Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { profile.Id } }
                };
                
                ShowAcceptButtonToClientRpc(senderId, onlyToTarget);
            }
        }
        
        [ClientRpc]
        private void ShowAcceptButtonToClientRpc(ulong requesterId, ClientRpcParams clientRpcParams = default)
        {
            _acceptSwapRoleButton.gameObject.SetActive(true);
            _acceptSwapRoleButton.onClick.RemoveAllListeners();
            _acceptSwapRoleButton.onClick.AddListener(() => AcceptSwapRoleServerRpc(requesterId));
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void AcceptSwapRoleServerRpc(ulong requesterId, ServerRpcParams rpcParams = default)
        {
            _swapRoleService.SwapRoleBetween(requesterId, rpcParams.Receive.SenderClientId);

            HideAcceptButtonClientRpc();
            ShowSwapRoleButtonClientRpc();
        }
        
        [ClientRpc]
        private void HideAcceptButtonClientRpc() => 
            _acceptSwapRoleButton.gameObject.SetActive(false);
        
        [ClientRpc]
        private void HideSwapRoleButtonClientRpc() => 
            _swapRoleButton.gameObject.SetActive(false);
        
        [ClientRpc]
        private void ShowSwapRoleButtonClientRpc() => 
            _swapRoleButton.gameObject.SetActive(true);
        
        [ClientRpc]
        private void ShowReadyButtonClientRpc() => 
            _readyButton.gameObject.SetActive(true);
        
        [ClientRpc]
        private void HideReadyButtonClientRpc() => 
            _readyButton.gameObject.SetActive(false);

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
            _acceptSwapRoleButton.onClick.RemoveAllListeners();
        }
    }
}