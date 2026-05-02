using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Code.Runtime.CommonServices.RolePicker;
using _Project.Code.Runtime.CommonServices.WindowManagement;
using _Project.Code.Runtime.Configs.Camera;
using _Project.Code.Runtime.Configs.Character;
using _Project.Code.Runtime.Configs.Game;
using _Project.Code.Runtime.Configs.Slots;
using _Project.Code.Runtime.Configs.Window;
using _Project.Code.Runtime.Infrustructure.AssetsManagement;
using UnityEngine;

namespace _Project.Code.Runtime.CommonServices.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private readonly IAssetsProvider _assetsProvider;
        
        private Dictionary<WindowId, GameObject> _windowPrefabsById;
        private Dictionary<GameRole, CharacterConfigSO> _characterConfigsByRole;
        private SlotsDataHolderSO _lobbySlotsDataHolder;
        private SlotsDataHolderSO _levelSlotsDataHolder;
        private GameConfigSO _gameConfig;
        private CameraConfigSO _cameraConfig;
        
        public GameConfigSO GameConfig => _gameConfig;
        public CameraConfigSO CameraConfig => _cameraConfig;
        public SlotsDataHolderSO LobbySlotsDataHolder => _lobbySlotsDataHolder;
        public SlotsDataHolderSO LevelSlotsDataHolder => _levelSlotsDataHolder;

        public StaticDataService(IAssetsProvider assetsProvider) => 
            _assetsProvider = assetsProvider;

        public void Initialize()
        {
            _gameConfig = _assetsProvider.Load<GameConfigSO>(AssetsPath.GameConfig);
            _cameraConfig = _assetsProvider.Load<CameraConfigSO>(AssetsPath.CameraConfig);
            _lobbySlotsDataHolder = _assetsProvider.Load<SlotsDataHolderSO>(AssetsPath.LobbySlotsDataHolder);
            _levelSlotsDataHolder = _assetsProvider.Load<SlotsDataHolderSO>(AssetsPath.LevelSlotsDataHolder);
            
            _windowPrefabsById = _assetsProvider.Load<WindowConfigsSO>(AssetsPath.WindowConfigs)
                .WindowConfigs
                .ToDictionary(x => x.Id, x => x.Prefab);

            _characterConfigsByRole = _assetsProvider.LoadAll<CharacterConfigSO>(AssetsPath.CharacterConfigs)
                .ToDictionary(x => x.ForRole, x => x);
        }

        public CharacterConfigSO GetCharacterConfigForRole(GameRole role) =>
            _characterConfigsByRole.TryGetValue(role, out CharacterConfigSO config)
            ? config
            : throw new Exception($"Character config for role {role} was not found");
        
        public GameObject GetWindowPrefab(WindowId id) =>
            _windowPrefabsById.TryGetValue(id, out GameObject prefab)
                ? prefab
                : throw new Exception($"Prefab config for window {id} was not found");
    }
}