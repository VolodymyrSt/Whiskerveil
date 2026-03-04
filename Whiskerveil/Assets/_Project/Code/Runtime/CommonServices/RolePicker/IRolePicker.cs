using _Project.Code.Runtime.Character;

namespace _Project.Code.Runtime.CommonServices.RolePicker
{
    public interface IRolePicker
    {
        void PickRoleForEachPlayers(ICharacter[] characters);
    }
}