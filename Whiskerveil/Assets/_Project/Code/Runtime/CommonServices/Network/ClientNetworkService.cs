using System.Text;
using _Project.Code.Runtime.Infrustructure;
using _Project.Code.Runtime.Utils;
using Cysharp.Threading.Tasks;
using Unity.Netcode;

namespace _Project.Code.Runtime.CommonServices.Network
{
    public class ClientNetworkService : IClientNetworkService
    {
        private readonly LoadingCurtain _loadingCurtain;
        
        private bool _isConnecting;
        
        public ClientNetworkService(LoadingCurtain loadingCurtain) => 
            _loadingCurtain = loadingCurtain;

        public async UniTask StartClient(string nickname)
        {
            // _loadingCurtain.Procced();
            
            if (_isConnecting)
                return;

            _isConnecting = true;
            
            var net = NetworkManager.Singleton;
            
            if (net.IsListening)
            {
                net.Shutdown();
                await UniTask.WaitUntil(() => !net.IsListening);
            }
            
            NetworkManager.Singleton.NetworkConfig.ConnectionData =
                Encoding.UTF8.GetBytes(nickname);
            
            NetworkManager.Singleton.StartClient();
            
            _isConnecting = false;
        }
    }
}