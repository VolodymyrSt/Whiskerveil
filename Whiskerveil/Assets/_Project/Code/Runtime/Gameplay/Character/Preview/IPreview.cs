using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.Gameplay.Character.View;
using UnityEngine;

namespace _Project.Code.Runtime.Gameplay.Character.Preview
{
    public interface IPreview
    {
        Transform Transform { get; }
        ICharacterView View { get; }
        GameRole Role { get; }
        void SetReadyInLobby(bool ready);
        void SetName(string characterName);
        void AssignRole(GameRole role);
    }
}