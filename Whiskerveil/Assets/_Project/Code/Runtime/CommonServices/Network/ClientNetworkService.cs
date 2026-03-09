using System.Text;
using _Project.Code.Runtime.Infrustructure;
using Unity.Netcode;

namespace _Project.Code.Runtime.CommonServices.Network
{
    public class ClientNetworkService : IClientNetworkService
    {
        private readonly LoadingCurtain _loadingCurtain;
        
        public ClientNetworkService(LoadingCurtain loadingCurtain) => 
            _loadingCurtain = loadingCurtain;

        public void StartClient(string nickname)
        {
            // _loadingCurtain.Procced();
            
            NetworkManager.Singleton.NetworkConfig.ConnectionData =
                Encoding.UTF8.GetBytes(nickname);
            
            NetworkManager.Singleton.StartClient();
        }
    }
}