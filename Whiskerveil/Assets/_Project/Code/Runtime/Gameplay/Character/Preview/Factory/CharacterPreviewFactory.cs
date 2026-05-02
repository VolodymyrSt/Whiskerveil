using System;
using _Project.Code.Runtime.CommonServices.ClientRegistry;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.Infrustructure.AssetsManagement;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.Gameplay.Character.Preview.Factory
{
    //Used for visual demonstration (In lobby)
    public class CharacterPreviewFactory : ICharacterPreviewFactory
    {
        private readonly IAssetsProvider _assetsProvider;
        private readonly DiContainer _container;
        
        public CharacterPreviewFactory(IAssetsProvider assetsProvider, DiContainer container)
        {
            _assetsProvider = assetsProvider;
            _container = container;
        }
        
        public IPreview CreatePreviewByProfile(ClientProfile profile, Vector3 at, Quaternion atRot) =>
            CreatePreview(profile.Id, profile.Name.ToString(), profile.Role, at, atRot);
        
        public IPreview CreatePreview(ulong clientId, string clientName, GameRole @as, Vector3 at, Quaternion atRot)
        {
            if (!NetworkManager.Singleton.IsServer)
                throw new Exception("Only server can create preview.");
            
            var character = _assetsProvider.Instantiate<CharacterPreview>(AssetsPath.CharacterPreviewPath, at, atRot);
            character.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            character.AssignRole(@as);
            character.SetName(clientName);
            
            return character;
        }
    }
}