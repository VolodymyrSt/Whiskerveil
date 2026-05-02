using _Project.Code.Runtime.CommonServices.RolePicker;

namespace _Project.Code.Runtime.Gameplay.Character.View
{
    public interface ICharacterView
    {
        void UpdateName(string characterName);
        void SwitchViewBaseOnRole(GameRole role);
    }
}