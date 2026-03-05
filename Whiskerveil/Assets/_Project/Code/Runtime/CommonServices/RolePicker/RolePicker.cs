using System.Collections.Generic;
using _Project.Code.Runtime.Character;
using UnityEngine;

namespace _Project.Code.Runtime.CommonServices.RolePicker
{
    public class RolePicker : IRolePicker
    {
        private readonly List<RoleSlot> _roleSlots = new() {
            new RoleSlot(GameRole.Seeker, false),
            new RoleSlot(GameRole.Hider, false),
            new RoleSlot(GameRole.Hider, false),
            new RoleSlot(GameRole.Hider, false)
        };

        public GameRole GetNextAvailableRole()
        {
            RoleSlot roleSlot = _roleSlots.Find(x => !x.IsTaken);
            roleSlot.IsTaken = true;
            return roleSlot.Role;
        }

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

    public class RoleSlot
    {
        public readonly GameRole Role;
        public bool IsTaken;

        public RoleSlot(GameRole role, bool isTaken)
        {
            Role = role;
            IsTaken = isTaken;
        }
    }

    public enum GameRole {
        None, Hider, Seeker
    }
}