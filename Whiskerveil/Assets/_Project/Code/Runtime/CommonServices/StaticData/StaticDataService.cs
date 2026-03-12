using _Project.Code.Runtime.Configs.Game;

namespace _Project.Code.Runtime.CommonServices.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private GameConfigSO _gameConfig;
        
        public GameConfigSO GameConfig => _gameConfig;
        
        public StaticDataService(GameConfigSO gameConfig) => 
            _gameConfig = gameConfig;
    }
}