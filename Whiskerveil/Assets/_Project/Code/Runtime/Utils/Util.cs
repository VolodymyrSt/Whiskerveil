namespace _Project.Code.Runtime.Utils
{
    public static class Util
    {
        public static string GetUniqueId() =>
            System.Guid.NewGuid().ToString();
    }
}