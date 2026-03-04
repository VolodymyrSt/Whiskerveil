using System;
using _Project.Code.Runtime.CommonServices.HostLobbyState;
using _Project.Code.Runtime.CommonServices.LobbySlots;
using _Project.Code.Runtime.CommonServices.SceneLoader;
using _Project.Code.Runtime.Infrustructure;
using Unity.Netcode;

namespace _Project.Code.Runtime.CommonServices.Network
{
    public class HostNetworkService : IHostNetworkService, IDisposable
    {
        public event Action<ulong> OnClientJoinedLobby;
        public event Action<ulong> OnHostJoinedLobby;
            
        private readonly ISceneLoader _sceneLoader;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly ILobbySlotService _lobbySlotService;
        private readonly IHostLobbyStateService _hostLobbyStateService;
        
        public HostNetworkService(ISceneLoader sceneLoader, LoadingCurtain loadingCurtain
            , IHostLobbyStateService hostLobbyStateService, ILobbySlotService lobbySlotService)
        {
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
            _hostLobbyStateService = hostLobbyStateService;
            _lobbySlotService = lobbySlotService;
        }
        
        public void StartHost()
        {
            // _loadingCurtain.Procced();
            
            NetworkManager.Singleton.StartHost();
            
            _lobbySlotService.PrepareSlots();
            
            _sceneLoader.LoadSync(SceneList.Lobby, () => {
                OnHostJoinedLobby?.Invoke(NetworkManager.Singleton.LocalClientId);

                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            });
        }
        
        private void OnClientConnected(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer) return;
            
            OnClientJoinedLobby?.Invoke(clientId);
        }
        
        private void OnClientDisconnect(ulong clientId)
        {
            
        }
        
        public void Dispose()
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer) return;
            
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }
}