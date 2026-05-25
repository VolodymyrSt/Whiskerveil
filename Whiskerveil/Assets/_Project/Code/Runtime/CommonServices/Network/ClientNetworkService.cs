using System.Text;
using _Project.Code.Runtime.Infrustructure;
using _Project.Code.Runtime.Utils;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

namespace _Project.Code.Runtime.CommonServices.Network
{
    public class ClientNetworkService : IClientNetworkService
    {
        private readonly LoadingCurtain _loadingCurtain;
        private readonly UnityTransport _transport;
        
        private bool _isConnecting;
        
        public ClientNetworkService(LoadingCurtain loadingCurtain, UnityTransport transport)
        {
            _loadingCurtain = loadingCurtain;
            _transport = transport;
        }

        public async UniTask StartClient(string nickname, string joinCode)
        {
            _loadingCurtain.Procced();
            
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

            JoinAllocation relayAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayData = relayAllocation.ToRelayServerData("dtls");

            _transport.SetRelayServerData(relayData);
            
            NetworkManager.Singleton.StartClient();
            
            _isConnecting = false;
        }
    }
}