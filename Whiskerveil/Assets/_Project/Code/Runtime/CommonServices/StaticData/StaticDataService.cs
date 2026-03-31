using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Code.Runtime.CommonServices.WindowManagement;
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
        private SlotsDataHolderSO _lobbySlotsDataHolder;
        private SlotsDataHolderSO _levelSlotsDataHolder;
        private GameConfigSO _gameConfig;
        
        public GameConfigSO GameConfig => _gameConfig;
        public SlotsDataHolderSO LobbySlotsDataHolder => _lobbySlotsDataHolder;
        public SlotsDataHolderSO LevelSlotsDataHolder => _levelSlotsDataHolder;

        public StaticDataService(IAssetsProvider assetsProvider) => 
            _assetsProvider = assetsProvider;

        public void Initialize()
        {
            _gameConfig = _assetsProvider.Load<GameConfigSO>(AssetsPath.GameConfig);
            _lobbySlotsDataHolder = _assetsProvider.Load<SlotsDataHolderSO>(AssetsPath.LobbySlotsDataHolder);
            _levelSlotsDataHolder = _assetsProvider.Load<SlotsDataHolderSO>(AssetsPath.LevelSlotsDataHolder);
            _windowPrefabsById = _assetsProvider.Load<WindowConfigsSO>(AssetsPath.WindowConfigs)
                .WindowConfigs
                .ToDictionary(x => x.Id, x => x.Prefab);
        }
        
        public GameObject GetWindowPrefab(WindowId id) =>
            _windowPrefabsById.TryGetValue(id, out GameObject prefab)
                ? prefab
                : throw new Exception($"Prefab config for window {id} was not found");
    }
}