using System;
using _Project.Code.Runtime.CommonServices.ClientRegistry;

namespace _Project.Code.Runtime.CommonServices.GameState
{
    public interface IGameStateService
    {
        event Action OnAllClientReadyToPlay;
        SceneState CurrentSceneState { get; }
        void SetSceneState(SceneState sceneState);
        void UpdateClientState(ulong clientId, bool isReadyToPlay);
        void ClearClientLobbyStates();
        void RemoveClientLobbyState(ulong clientId);
        void AddClientLobbyState(ClientProfile profile);
        event Action<int> OnLobbyStateChanged;
    }
}