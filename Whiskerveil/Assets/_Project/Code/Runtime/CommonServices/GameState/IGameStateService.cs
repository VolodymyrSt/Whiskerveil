using System;
using System.Collections.Generic;
using _Project.Code.Runtime.Character;
using _Project.Code.Runtime.CommonServices.ClientRegistry;

namespace _Project.Code.Runtime.CommonServices.GameState
{
    public interface IGameStateService
    {
        event Action OnAllClientReadyToPlay;
        SceneState CurrentSceneState { get; }
        List<ClientLobbyState> LobbyStates { get; }
        void SetSceneState(SceneState sceneState);
        ClientLobbyState UpdateClientLobbyState(ulong clientId);
        void PrepairForClientConnection();
        void RemoveClientLobbyState(ulong clientId);
        void AddClientLobbyState(ClientProfile profile, ICharacter character);
        event Action<int> OnLobbyStateChanged;
        ClientLobbyState GetClientLobbyStateById(ulong clientId);
    }
}