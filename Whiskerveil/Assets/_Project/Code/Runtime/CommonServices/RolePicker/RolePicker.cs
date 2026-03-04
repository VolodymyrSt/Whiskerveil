using _Project.Code.Runtime.Character;
using UnityEngine;

namespace _Project.Code.Runtime.CommonServices.RolePicker
{
    public class RolePicker : IRolePicker
    {
        public void PickRoleForEachPlayers(ICharacter[] characters)
        {
            if (characters.Length < 2)
            {
                Debug.LogWarning("Active count of players are too small to pick a roles");
                return;
            }
            
            var randomIndex = UnityEngine.Random.Range(0, characters.Length);

            for (int i = 0; i < characters.Length; i++)
            {
                if (i == randomIndex)
                    characters[i].AssignRole(GameRole.Seeker);
                else
                    characters[i].AssignRole(GameRole.Hider);
            }
        }
    }

    public enum GameRole {
        None, Hider, Seeker
    }
}