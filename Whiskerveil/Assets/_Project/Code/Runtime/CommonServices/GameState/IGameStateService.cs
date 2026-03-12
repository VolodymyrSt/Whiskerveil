using System;
using _Project.Code.Runtime.Character;
using _Project.Code.Runtime.CommonServices.ClientRegistry;

namespace _Project.Code.Runtime.CommonServices.GameState
{
    public interface IGameStateService
    {
        event Action OnAllClientReadyToPlay;
        SceneState CurrentSceneState { get; }
        void SetSceneState(SceneState sceneState);
        ClientLobbyState UpdateClientLobbyState(ulong clientId);
        void ClearClientLobbyStates();
        void RemoveClientLobbyState(ulong clientId);
        void AddClientLobbyState(ClientProfile profile, ICharacter character);
        event Action<int> OnLobbyStateChanged;
    }
}