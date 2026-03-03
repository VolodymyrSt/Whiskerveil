using System;
using System.Collections.Generic;
using _Project.Code.Runtime.Character;
using _Project.Code.Runtime.Character.Factory;
using _Project.Code.Runtime.CommonServices.RolePickerService;
using _Project.Code.Runtime.CommonServices.SceneLoaderService;
using _Project.Code.Runtime.Infrustructure;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Code.Runtime.CommonServices.NetworkServices
{
    public class HostNetworkService : IHostNetworkService, IDisposable
    {
        public event Action<ulong, ICharacter> OnClientJoinedLobby;
        public event Action<ulong, ICharacter> OnHostJoinedLobby;
            
        private readonly ISceneLoader _sceneLoader;
        private readonly ICharacterFactory _characterFactory;
        private readonly LoadingCurtain _loadingCurtain;
        
        public HostNetworkService(ISceneLoader sceneLoader, ICharacterFactory characterFactory
            , LoadingCurtain loadingCurtain)
        {
            _sceneLoader = sceneLoader;
            _characterFactory = characterFactory;
            _loadingCurtain = loadingCurtain;
        }
        
        public void StartHost()
        {
            _loadingCurtain.Procced();
            
            NetworkManager.Singleton.StartHost();
            
            _sceneLoader.LoadSync(SceneList.Lobby, () => {
                CreateHostPlayerPrefab();
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            });
        }
        
        private void OnClientConnected(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer) return;
            
            CreateClientPlayerPrefab(clientId);
        }
        
        private void OnClientDisconnect(ulong clientId)
        {
            
        }
        
        private void CreateClientPlayerPrefab(ulong clientId)
        {
            var character = _characterFactory.CreateCharacter(clientId, GameRole.Hider);
            OnClientJoinedLobby?.Invoke(clientId, character);
        }
        
        private void CreateHostPlayerPrefab()
        {
            ulong hostId = NetworkManager.Singleton.LocalClientId;
            var character = _characterFactory.CreateCharacter(hostId, GameRole.Seeker);
            OnHostJoinedLobby?.Invoke(hostId, character);
        }
        
        public void Dispose()
        {
            if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer) return;
            
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }
}