using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Code.Runtime.Character;
using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.Utils;
using Unity.Netcode;

namespace _Project.Code.Runtime.CommonServices.GameState
{
    public enum SceneState {InLobby, InLevel, None}

    public class GameStateService : IGameStateService
    {
        public event Action OnAllClientReadyToPlay;
        public event Action<int> OnLobbyStateChanged;
        
        private readonly List<ClientLobbyState> _clientLobbyStates = new();
        private SceneState _currentSceneState = SceneState.None;
        
        public SceneState CurrentSceneState => _currentSceneState;

        public void RemoveClientLobbyState(ulong clientId)
        {
            if (!Net.IsServer) return;

            var state = GetClientLobbyStateById(clientId);
            _clientLobbyStates.Remove(state);
            
            OnLobbyStateChanged?.Invoke(_clientLobbyStates.Count);
        }

        public void AddClientLobbyState(ClientProfile profile, ICharacter character)
        {
            if (!Net.IsServer) return;
            
            _clientLobbyStates.Add(new ClientLobbyState 
                { ClientId = profile.Id, IsReadyToPlay = false, Character = character });
            
            OnLobbyStateChanged?.Invoke(_clientLobbyStates.Count);
        }
        
        public void ClearClientLobbyStates()
        {
            if (!Net.IsServer) return;
            
            _clientLobbyStates.Clear();
            
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
            state.Character.SetReadyInLobby(state.IsReadyToPlay);
            
            if (IsAllClientsReadyToPlay())
                OnAllClientReadyToPlay?.Invoke();

            return state;
        }

        private bool IsAllClientsReadyToPlay()
        {
            foreach (var clientLobbyState in _clientLobbyStates)
                if (!clientLobbyState.IsReadyToPlay)
                    return false;
            
            return true;
        }

        private ClientLobbyState GetClientLobbyStateById(ulong clientId)
        {
            ClientLobbyState state = _clientLobbyStates.Find(x => x.ClientId == clientId);

            if (state != null)
                return state; 
            
            throw new Exception($"ClientLobbyState not found for id: {clientId}");
        }
        
    }
}
