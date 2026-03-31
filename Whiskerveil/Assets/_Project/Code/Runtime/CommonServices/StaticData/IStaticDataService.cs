using _Project.Code.Runtime.CommonServices.WindowManagement;
using _Project.Code.Runtime.Configs.Game;
using _Project.Code.Runtime.Configs.Slots;
using UnityEngine;

namespace _Project.Code.Runtime.CommonServices.StaticData
{
    public interface IStaticDataService
    {
        GameConfigSO GameConfig { get; }
        SlotsDataHolderSO LobbySlotsDataHolder { get; }
        SlotsDataHolderSO LevelSlotsDataHolder { get; }
        GameObject GetWindowPrefab(WindowId id);
        void Initialize();
    }
}