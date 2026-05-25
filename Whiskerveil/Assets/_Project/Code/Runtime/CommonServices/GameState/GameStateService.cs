using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.Gameplay.Character;
using _Project.Code.Runtime.Gameplay.Character.Preview;
using _Project.Code.Runtime.Utils;
using Unity.Netcode;

namespace _Project.Code.Runtime.CommonServices.GameState
{
    public enum SceneState {InLobby, InLevel, None}

    public class GameStateService : IGameStateService
    {
        public event Action OnAllClientReadyToPlay;
        public event Action<int> OnLobbyStateChanged;
        public event Action OnAllHidersAreDead;
        
        private readonly List<ClientLobbyState> _clientLobbyStates = new();
        private readonly List<ClientGameplayState> _clientGameplayStates = new();
        private SceneState _currentSceneState = SceneState.None;
        
        public List<ClientLobbyState> LobbyStates => _clientLobbyStates;
        public List<ClientGameplayState> GameplayStates => _clientGameplayStates;
        public SceneState CurrentSceneState => _currentSceneState;
        
        public void RemoveClientLobbyState(ulong clientId)
        {
            if (!Net.IsServer) return;

            var state = GetClientLobbyStateById(clientId);
            _clientLobbyStates.Remove(state);
            
            OnLobbyStateChanged?.Invoke(_clientLobbyStates.Count);
        }

        public void AddClientLobbyState(ClientProfile profile, IPreview character)
        {
            if (!Net.IsServer) return;
            
            _clientLobbyStates.Add(new ClientLobbyState 
                { ClientId = profile.Id, IsReadyToPlay = false, Preview = character });
            
            OnLobbyStateChanged?.Invoke(_clientLobbyStates.Count);
        }

        public bool IsAllHidersDead()
        {
            var allHiders = _clientGameplayStates.FindAll(x => x != null && x.Role == GameRole.Hider);

            foreach (var hider in allHiders)
                if (!hider.IsDead) return false;
            
            return true;
        }
        
        public ClientGameplayState SetClientGameplayStateToDead(ulong clientId)
        {
            var client = _clientGameplayStates.Find(x => x.ClientId == clientId);
            
            if (client == null)
                throw new NullReferenceException($"Client {clientId} does not exist in Gameplay States");

            client.IsDead = true;

            if (IsAllHidersDead())
                OnAllHidersAreDead?.Invoke();

            return client;
        }
        
        public bool IsClientByIdDead(ulong clientId) => 
            GetClientGameplayStateById(clientId).IsDead;

        public void AddClientGameplayState(ClientProfile profile, ICharacter character)
        {
            if (!Net.IsServer) return;
            
            _clientGameplayStates.Add(new ClientGameplayState() 
                { ClientId = profile.Id, IsDead = false, Role = profile.Role, Character = character});
        }
        
        public ClientGameplayState GetSeekerGameplayState()
        {
            if (!Net.IsServer) return null;
            
            return _clientGameplayStates.Find(x => x!= null && x.Role == GameRole.Seeker);
        }
        
        public void RemoveClientGameplayState(ulong clientId)
        {
            if (!Net.IsServer) return;

            var state = GetClientGameplayStateById(clientId);
            _clientGameplayStates.Remove(state);
        }
        
        public void PrepairForClientConnection()
        {
            if (!Net.IsServer) return;
            
            _clientLobbyStates.Clear();
            _clientGameplayStates.Clear();
            
            OnLobbyStateChanged?.Invoke(_clientLobbyStates.Count);
        }

        public void SetSceneState(SceneState sceneState)
        {
            if (!Net.IsServer) return;
            _currentSceneState = sceneState;
        }

        public ClientLobbyState UpdateClientLobbyState(ulong clientId)
        {
            if (!Net.IsServer) return null;
            
            ClientLobbyState state = GetClientLobbyStateById(clientId);
            state.IsReadyToPlay = !state.IsReadyToPlay;
            state.Preview.SetReadyInLobby(state.IsReadyToPlay);
            
            if (IsAllClientsReadyToPlay())
                OnAllClientReadyToPlay?.Invoke();

            return state;
        }
        
        public ClientLobbyState GetClientLobbyStateById(ulong clientId)
        {
            ClientLobbyState state = _clientLobbyStates.Find(x => x.ClientId == clientId);

            if (state != null)
                return state; 
            
            throw new Exception($"ClientLobbyState not found for id: {clientId}");
        }
        
        public ClientGameplayState GetClientGameplayStateById(ulong clientId)
        {
            ClientGameplayState state = _clientGameplayStates.Find(x => x.ClientId == clientId);

            if (state != null)
                return state; 
            
            throw new Exception($"ClientLobbyState not found for id: {clientId}");
        }

        private bool IsAllClientsReadyToPlay()
        {
            foreach (var clientLobbyState in _clientLobbyStates)
                if (!clientLobbyState.IsReadyToPlay)
                    return false;
            
            return true;
        }
    }
}
