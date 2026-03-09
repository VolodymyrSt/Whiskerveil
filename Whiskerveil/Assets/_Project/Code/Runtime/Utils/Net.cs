using Unity.Netcode;

namespace _Project.Code.Runtime.Utils
{
    public static class Net
    {
        public static bool IsServer =>
            NetworkManager.Singleton.IsServer;
        
        public static bool IsClient =>
            NetworkManager.Singleton.IsClient;
    }
}