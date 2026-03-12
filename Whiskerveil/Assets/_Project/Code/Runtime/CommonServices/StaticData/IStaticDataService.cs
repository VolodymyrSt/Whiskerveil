using _Project.Code.Runtime.Configs.Game;

namespace _Project.Code.Runtime.CommonServices.StaticData
{
    public interface IStaticDataService
    {
        GameConfigSO GameConfig { get; }
    }
}