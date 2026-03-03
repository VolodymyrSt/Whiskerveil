using _Project.Code.Runtime.CommonServices.SceneLoaderService;
using _Project.Code.Runtime.Infrustructure;
using Unity.Netcode;

namespace _Project.Code.Runtime.CommonServices.NetworkServices
{
    public class ClientNetworkService : IClientNetworkService
    {
        private readonly LoadingCurtain _loadingCurtain;
        
        public ClientNetworkService(LoadingCurtain loadingCurtain) => 
            _loadingCurtain = loadingCurtain;

        public void StartClient()
        {
            _loadingCurtain.Procced();
            NetworkManager.Singleton.StartClient();
        }
    }
}