using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.GameState;
using _Project.Code.Runtime.CommonServices.Network;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.SwapRole;
using _Project.Code.Runtime.CommonServices.WindowManagement;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Code.Runtime.Gameplay.UI.Lobby
{
    public class LobbyUIMediator : NetworkBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _readyButton;
        [SerializeField] private Button _swapRoleButton;
        [SerializeField] private Button _settingButton;
        [SerializeField] private Button _leaveLobbyButton;
        
        [Header("Windows")]
        [SerializeField] private SwapRoleWindow _swapRoleWindow;
        
        [Header("Other")]
        [SerializeField] private Image _readySigh;
        
        private IClientsRegistry _clientsRegistry;
        private ISwapRoleService _swapRoleService;
        private IGameStateService _gameStateService;
        private IHostNetworkService _hostNetworkService;
        private IWindowService _windowsService;
        
        [Inject]
        public void Construct(IClientsRegistry clientsRegistry, ISwapRoleService swapRoleService,
            IGameStateService gameStateService, IHostNetworkService hostNetworkService
            , IWindowService windowsService)
        {
            _clientsRegistry = clientsRegistry;
            _swapRoleService = swapRoleService;
            _gameStateService = gameStateService;
            _hostNetworkService = hostNetworkService;
            _windowsService = windowsService;
        }
        
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _gameStateService.OnLobbyStateChanged += OnLobbyStateChanged;
                _hostNetworkService.OnClientDisconnected += OnClientDisconnected;
                _hostNetworkService.OnClientConnected += OnClientConnected;
            }

            _readySigh.gameObject.SetActive(false);
            _swapRoleWindow.Hide();
            
            _readyButton.onClick.AddListener(() => OnReadyButtonPressedServerRpc());
            _swapRoleButton.onClick.AddListener(() => RequestSwapRoleServerRpc());
            _leaveLobbyButton.onClick.AddListener(() => {
                _leaveLobbyButton.gameObject.SetActive(false);
                _windowsService.Open(WindowId.LeaveLobby, () => _leaveLobbyButton.gameObject.SetActive(true));
            });
        }

        private void OnClientConnected(ulong clientId)
        {
            if (_swapRoleService.HasRequester) {
                HideSwapRoleButtonClientRpc(new ClientRpcParams() {
                    Send = new ClientRpcSendParams { TargetClientIds = new ulong[] {clientId} }
                });
            }
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (_swapRoleService.HasRequester)
            {
                if (_swapRoleService.IsRequester(clientId))
                    CleanUpClientDisconnectionWithPendingRequest(clientId);
                else if (_swapRoleService.IsApprover(clientId))
                    CleanUpClientDisconnectionWithUnAnsweredApprove(clientId);
            }
        }

        
        [ServerRpc(RequireOwnership = false)]
        private void OnReadyButtonPressedServerRpc(ServerRpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;
            
            if (_swapRoleService.HasRequester && _swapRoleService.IsRequester(clientId))
            {
                Debug.Log("Can`t cose your are swaping role");
                return;
            }
            
            ClientLobbyState updatedState = _gameStateService.UpdateClientLobbyState(clientId);
            
            if (!_swapRoleService.HasRequester)
                ToggleSwapRoleButtonFor(rpcParams.Receive.SenderClientId, updatedState.IsReadyToPlay);

            OnReadyButtonPressedClientRpc(updatedState.IsReadyToPlay, new ClientRpcParams {
                Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { clientId } }
            });
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestSwapRoleServerRpc(ServerRpcParams rpcParams = default)
        {
            ulong senderId = rpcParams.Receive.SenderClientId;
            ClientProfile requesterProfile = _clientsRegistry.GetById(senderId);
            
            _swapRoleService.ClearRequest();
            _swapRoleService.AssignRequester(senderId);
            
            HideSwapRoleButtonClientRpc();
            
            foreach (ClientProfile targetProfile in _clientsRegistry.Profiles)
            {
                if (targetProfile.Id == senderId) continue;
                if (targetProfile.Role == requesterProfile.Role) continue;
                _swapRoleService.AddApprover(targetProfile.Id);
                
                ShowSwapRoleWindowToClientRpc(requesterProfile.Name, requesterProfile.Role, senderId, new ClientRpcParams {
                    Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { targetProfile.Id } }
               });
            }
        }
        
        [ClientRpc]
        private void ShowSwapRoleWindowToClientRpc(FixedString64Bytes requesterName, GameRole requesterRole,
            ulong requesterId, ClientRpcParams clientRpcParams = default)
        {
            _swapRoleWindow.Show(requesterName, requesterRole, 
                onAccepted: () => AcceptSwapRoleServerRpc(requesterId), 
                onDeclined: () => DeclineSwapRoleServerRpc());
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void AcceptSwapRoleServerRpc(ulong requesterId, ServerRpcParams rpcParams = default)
        {
            ulong accepterId = rpcParams.Receive.SenderClientId;
            ClientLobbyState accepterState = _gameStateService.GetClientLobbyStateById(accepterId);

            if (accepterState.IsReadyToPlay)
            {
                Debug.Log("Can`t accept cose your are Ready To Play");
                return;
            }
            
            _swapRoleService.SwapRoleBetween(requesterId, rpcParams.Receive.SenderClientId);
            _swapRoleService.ClearRequest();
            
            HideSwapRoleWindowClientRpc();
            ShowSwapRoleButtonClientRpc();
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void DeclineSwapRoleServerRpc(ServerRpcParams rpcParams = default)
        {
            ulong declinerId = rpcParams.Receive.SenderClientId;
            ClientLobbyState declinerState = _gameStateService.GetClientLobbyStateById(declinerId);
            
            if (declinerState.IsReadyToPlay)
            {
                Debug.Log("Can`t decline cose your are Ready To Play");
                return;
            }
            
            _swapRoleService.RemoveApprover(declinerId);

            if (!_swapRoleService.HasApprovers)
            {
                _swapRoleService.ClearRequest();
                
                foreach (ClientLobbyState lobbyState in _gameStateService.LobbyStates)
                    ToggleSwapRoleButtonFor(lobbyState.ClientId, lobbyState.IsReadyToPlay);
            }
            
            HideSwapRoleWindowClientRpc(new ClientRpcParams {
                Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { declinerId } }
            });
        }
        
        [ClientRpc]
        private void HideSwapRoleWindowClientRpc(ClientRpcParams rpcParams = default) => 
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
            {
                if (!_swapRoleService.HasRequester) 
                    ShowSwapRoleButtonClientRpc();
            }
            else
                HideSwapRoleButtonClientRpc();
        }
        
        private void CleanUpClientDisconnectionWithPendingRequest(ulong clientId)
        {
            Debug.Log("ClientDisconnectedWithPendingRequest: client id" + clientId);
            _swapRoleService.ClearRequest();
                
            foreach (ClientLobbyState lobbyState in _gameStateService.LobbyStates)
                ToggleSwapRoleButtonFor(lobbyState.ClientId, lobbyState.IsReadyToPlay);

            HideSwapRoleWindowClientRpc();
        }
        
        private void CleanUpClientDisconnectionWithUnAnsweredApprove(ulong clientId)
        {
            Debug.Log("ClientDisconnectionWithUnAnsweredApprove: client id" + clientId);
            _swapRoleService.RemoveApprover(clientId);
                    
            if (!_swapRoleService.HasApprovers)
            {
                Debug.Log("Has zero Approvers when ClientDisconnectionWithUnAnsweredApprove: client id" + clientId);
                _swapRoleService.ClearRequest();
                
                foreach (ClientLobbyState lobbyState in _gameStateService.LobbyStates)
                    ToggleSwapRoleButtonFor(lobbyState.ClientId, lobbyState.IsReadyToPlay);
            }
        }
        
        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                _gameStateService.OnLobbyStateChanged -= OnLobbyStateChanged;
                _hostNetworkService.OnClientDisconnected -= OnClientDisconnected;
                _hostNetworkService.OnClientConnected -= OnClientConnected;
            }

            _readyButton.onClick.RemoveAllListeners();
            _swapRoleButton.onClick.RemoveAllListeners();
            _leaveLobbyButton.onClick.RemoveAllListeners();
            _settingButton.onClick.RemoveAllListeners();
        }
    }
}
