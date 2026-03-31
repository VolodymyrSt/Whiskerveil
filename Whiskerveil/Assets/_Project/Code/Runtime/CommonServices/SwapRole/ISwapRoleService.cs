namespace _Project.Code.Runtime.CommonServices.SwapRole
{
    public interface ISwapRoleService
    {
        void SwapRoleBetween(ulong fromClientId, ulong toClientId);
        bool HasRequester { get; }
        bool HasApprovers { get; }
        void AssignRequester(ulong requesterId);
        void AddApprover(ulong clientId);
        void RemoveApprover(ulong clientId);
        bool IsRequester(ulong requesterId);
        void ClearRequest();
        bool IsApprover(ulong approverId);
    }
}