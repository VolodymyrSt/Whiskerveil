using System;
using _Project.Code.Runtime.CommonServices.LobbySlots;
using _Project.Code.Runtime.CommonServices.PlayerRegistry;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.SceneLoader;
using _Project.Code.Runtime.Infrustructure;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Code.Runtime.CommonServices.Network
{
    public enum GameState {InLobby, InLevel, None}
    
    public class HostNetworkService : IHostNetworkService, IDisposable
    {
        public event Action<ulong> OnClientDisconnected;
        
        private readonly ISceneLoader _sceneLoader;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly ILobbySlotService _lobbySlotService;
        private readonly IPlayersRegistry _playersRegistry;
        private readonly IRolePicker _rolePicker;
        
        private GameState _gameState;
        
        public HostNetworkService(ISceneLoader sceneLoader, LoadingCurtain loadingCurtain
            , ILobbySlotService lobbySlotService, IPlayersRegistry playersRegistry, IRolePicker rolePicker)
        {
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
            _lobbySlotService = lobbySlotService;
            _playersRegistry = playersRegistry;
            _rolePicker = rolePicker;

            _gameState = GameState.None;
        }
        
        public void StartHost()
        {
            // _loadingCurtain.Procced();
            
            NetworkManager.Singleton.StartHost();
            _playersRegistry
                .AddProfile(new PlayerProfile(NetworkManager.Singleton.LocalClientId)
                .WithRole(_rolePicker.GetNextAvailableRole()));
            
            _lobbySlotService.PrepareSlots();
            
            _sceneLoader.LoadSync(SceneList.Lobby, () => {
                _gameState = GameState.InLobby;
                
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            });
        }
        
        private void OnClientConnected(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer) return;
            
            if (_gameState == GameState.InLevel)
                Debug.Log("Cant connect to server");
            
            _playersRegistry.AddProfile(new PlayerProfile(clientId)
                .WithRole(_rolePicker.GetNextAvailableRole()));
        }
        
        private void OnClientDisconnect(ulong clientId)
        {
            _playersRegistry.RemoveProfile(clientId);
            OnClientDisconnected?.Invoke(clientId);
            Debug.Log("Client disconnected");
        }

        public void Dispose()
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer) return;
            
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }
}