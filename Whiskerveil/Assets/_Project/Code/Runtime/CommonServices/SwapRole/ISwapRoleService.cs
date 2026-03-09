namespace _Project.Code.Runtime.CommonServices.SwapRole
{
    public interface ISwapRoleService
    {
        void SwapRoleBetween(ulong fromClientId, ulong toClientId);
    }
}