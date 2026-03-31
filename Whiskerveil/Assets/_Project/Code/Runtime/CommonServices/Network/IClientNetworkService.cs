using Cysharp.Threading.Tasks;

namespace _Project.Code.Runtime.CommonServices.Network
{
    public interface IClientNetworkService
    {
        UniTask StartClient(string nickname);
    }
}