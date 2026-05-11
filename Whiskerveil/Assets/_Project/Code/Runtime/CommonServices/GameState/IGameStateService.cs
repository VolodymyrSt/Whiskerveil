using System;
using System.Collections.Generic;
using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.Gameplay.Character.Preview;

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
        void AddClientLobbyState(ClientProfile profile, IPreview character);
        event Action<int> OnLobbyStateChanged;
        ClientLobbyState GetClientLobbyStateById(ulong clientId);
        bool IsAllHidersDead();
        void SetClientGameplayStateToDead(ulong clientId);
        void AddClientGameplayState(ClientProfile profile);
    }
}