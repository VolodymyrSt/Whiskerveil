using System;
using System.Collections.Generic;
using System.Text;
using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.GameState;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.SceneLoader;
using _Project.Code.Runtime.CommonServices.SlotsManagement;
using _Project.Code.Runtime.Infrustructure;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Code.Runtime.CommonServices.Network
{
    public class HostNetworkService : IHostNetworkService, IDisposable
    {
        public event Action<ulong> OnClientDisconnected;
        public event Action<ulong> OnClientConnected;
        
        private readonly Dictionary<ulong, string> _pendingNicknames = new();
        
        private readonly ISceneLoader _sceneLoader;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly ISlotService _slotService;
        private readonly IClientsRegistry _clientsRegistry;
        private readonly IGameStateService _gameStateService;
        private readonly IRolePicker _rolePicker;
        
        private bool _isStartingHost;
        
        public HostNetworkService(ISceneLoader sceneLoader, LoadingCurtain loadingCurtain
            , ISlotService slotService, IClientsRegistry clientsRegistry, IRolePicker rolePicker
            , IGameStateService gameStateService)
        {
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
            _slotService = slotService;
            _clientsRegistry = clientsRegistry;
            _rolePicker = rolePicker;
            _gameStateService = gameStateService;
        }
        
        public async UniTask StartHost(string nickname)
        {
            // _loadingCurtain.Procced();
            
            if (_isStartingHost)
                return;

            _isStartingHost = true;
            
            var net = NetworkManager.Singleton;
            
            if (net.IsListening)
            {
                net.Shutdown();
                await UniTask.WaitUntil(() => !net.IsListening);
            }
            
            net.ConnectionApprovalCallback = ApprovalCheck;
            net.StartHost();
            
            _rolePicker.RestoreAll();
            _slotService.Initialize();
            
            _clientsRegistry
                .AddProfile(new ClientProfile(net.LocalClientId)
                .WithName(nickname)
                .WithRole(_rolePicker.GetNextAvailableRole()));
            
            net.OnClientConnectedCallback += OnClientConnect;
            net.OnClientDisconnectCallback += OnClientDisconnect;
            
            _sceneLoader.LoadSync(SceneList.Lobby, () => {
                _gameStateService.SetSceneState(SceneState.InLobby);
            });
            
            _isStartingHost = false; 
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
        
        private void OnClientConnect(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer) return;
            
            if (_clientsRegistry.GetById(clientId) != null) //dublicate
                return;
            
            if (!_pendingNicknames.TryGetValue(clientId, out string nickname))
                nickname = $"Player{clientId}";
            
            _clientsRegistry.AddProfile(new ClientProfile(clientId)
                .WithName(nickname)
                .WithRole(_rolePicker.GetNextAvailableRole()));
            
            _pendingNicknames.Remove(clientId);
            OnClientConnected?.Invoke(clientId);
            Debug.Log($"Client connected: {clientId}, nickname: {nickname}");
        }
        
        private void OnClientDisconnect(ulong clientId)
        {
            if (_clientsRegistry.GetById(clientId) == null)
            {
                Debug.LogWarning($"[HostNetworkService] Client {clientId} disconnected before profile was registered, ignoring.");
                return;
            }
            
            OnClientDisconnected?.Invoke(clientId);
        }

        public void Dispose()
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer) return;
            
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            NetworkManager.Singleton.ConnectionApprovalCallback = null;
        }
    }
}