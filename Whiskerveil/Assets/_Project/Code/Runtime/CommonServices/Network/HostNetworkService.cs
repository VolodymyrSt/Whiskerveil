using System;
using System.Collections.Generic;
using System.Text;
using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.GameState;
using _Project.Code.Runtime.CommonServices.LobbySlots;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.SceneLoader;
using _Project.Code.Runtime.Infrustructure;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Code.Runtime.CommonServices.Network
{
    public class HostNetworkService : IHostNetworkService, IDisposable
    {
        public event Action<ulong> OnClientDisconnected;
        
        private readonly Dictionary<ulong, string> _pendingNicknames = new();
        
        private readonly ISceneLoader _sceneLoader;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly ILobbySlotService _lobbySlotService;
        private readonly IClientsRegistry _clientsRegistry;
        private readonly IGameStateService _gameStateService;
        private readonly IRolePicker _rolePicker;
        
        public HostNetworkService(ISceneLoader sceneLoader, LoadingCurtain loadingCurtain
            , ILobbySlotService lobbySlotService, IClientsRegistry clientsRegistry, IRolePicker rolePicker
            , IGameStateService gameStateService)
        {
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
            _lobbySlotService = lobbySlotService;
            _clientsRegistry = clientsRegistry;
            _rolePicker = rolePicker;
            _gameStateService = gameStateService;
        }
        
        public void StartHost(string nickname)
        {
            // _loadingCurtain.Procced();
            
            NetworkManager.Singleton.StartHost();
            _clientsRegistry
                .AddProfile(new ClientProfile(NetworkManager.Singleton.LocalClientId)
                .WithName(nickname)
                .WithRole(_rolePicker.GetNextAvailableRole()));
            
            _lobbySlotService.PrepareSlots();
            
            _sceneLoader.LoadSync(SceneList.Lobby, () => {
                _gameStateService.SetSceneState(SceneState.InLobby);
                
                NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            });
        }
        
        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            ulong clientId = request.ClientNetworkId;
            string nickname = request.Payload.Length > 0
                ? Encoding.UTF8.GetString(request.Payload)
                : $"Player {clientId}";
            
            _pendingNicknames[clientId] = nickname;

            response.Approved = true;
            response.CreatePlayerObject = false;
            response.Pending = false;
        }
        
        private void OnClientConnected(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer) return;
            
            if (!_pendingNicknames.TryGetValue(clientId, out string nickname))
                nickname = $"Player{clientId}";
            
            _clientsRegistry.AddProfile(new ClientProfile(clientId)
                .WithName(nickname)
                .WithRole(_rolePicker.GetNextAvailableRole()));
            
            _pendingNicknames.Remove(clientId);
            
            Debug.Log($"Client connected: {clientId}, nickname: {nickname}");
        }
        
        private void OnClientDisconnect(ulong clientId)
        {
            _clientsRegistry.RemoveProfile(clientId);
            OnClientDisconnected?.Invoke(clientId);
            Debug.Log("Client disconnected");
        }

        public void Dispose()
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer) return;
            
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
    }
}