using System;
using Unity.Netcode;
using Object = UnityEngine.Object;

namespace _Project.Code.Runtime.Utils
{
    public static class Util
    {
        public static string GetUniqueId() =>
            System.Guid.NewGuid().ToString();

        public static T GetComponentOnPlayerPrefab<T>(ulong clientId) where T : class
        {
            NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            
            if  (playerObject != null)
                return playerObject.GetComponent<T>();

            throw new Exception("Player not found");
        }
    }
}