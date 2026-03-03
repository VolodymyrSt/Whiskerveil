using _Project.Code.Runtime.Character;

namespace _Project.Code.Runtime.CommonServices.RolePickerService
{
    public interface IRolePicker
    {
        void PickRoleForEachPlayers(ICharacter[] characters);
    }
}