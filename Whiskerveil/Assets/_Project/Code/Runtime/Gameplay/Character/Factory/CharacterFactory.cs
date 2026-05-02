using System;
using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.Gameplay.Camera;
using _Project.Code.Runtime.Gameplay.Character.Preview;
using _Project.Code.Runtime.Infrustructure.AssetsManagement;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Gameplay.Character.Factory
{
    //Used for gameplay (In level)
    public class CharacterFactory : ICharacterFactory
    {
        private readonly IAssetsProvider _assetsProvider;
        private readonly DiContainer _container;
        
        public CharacterFactory(IAssetsProvider assetsProvider, DiContainer container)
        {
            _assetsProvider = assetsProvider;
            _container = container;
        }
        
        public ICharacter CreateCharacterByProfile(ClientProfile profile, Vector3 at, Quaternion atRot) =>
            CreateCharacter(profile.Id, profile.Name.ToString(), profile.Role, at, atRot);

        public ICharacter CreateCharacter(ulong clientId, string clientName, GameRole @as, Vector3 at, Quaternion atRot)
        {
            if (!NetworkManager.Singleton.IsServer)
                throw new Exception("Only server can create characters.");

            var character = _assetsProvider.Instantiate<Character>(AssetsPath.CharacterPath, at, atRot);
            _container.Inject(character);
            character.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            character.AssignRole(@as);
            character.SetName(clientName);
            return character;
        }
    }
}