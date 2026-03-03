using System;
using System.Collections.Generic;
using _Project.Code.Runtime.Character.View;
using _Project.Code.Runtime.CommonServices.RolePickerService;
using _Project.Code.Runtime.Infrustructure.AssetsManagement;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace _Project.Code.Runtime.Character.Factory
{
    public class CharacterFactory : ICharacterFactory
    {
        private readonly IAssetsProvider _assetsProvider;
        private readonly DiContainer _container;
        
        public CharacterFactory(IAssetsProvider assetsProvider) => 
            _assetsProvider = assetsProvider;

        public ICharacter CreateCharacter(ulong clientId, GameRole @as, Vector3 at)
        {
            if (!NetworkManager.Singleton.IsServer)
                throw new Exception("Only server can create characters.");

            var character = _assetsProvider.Instantiate<Character>(AssetsPath.CharacterPath, at);
            character.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            character.AssignRole(@as);
            
            return character;
        }
    }
}