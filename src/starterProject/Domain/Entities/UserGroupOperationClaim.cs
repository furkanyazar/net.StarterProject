namespace Domain.Entities;

public class UserGroupOperationClaim : Core.Security.Entities.UserGroupOperationClaim<int, int, int>
{
    public virtual UserGroup UserGroup { get; set; } = default!;
    public virtual OperationClaim OperationClaim { get; set; } = default!;
}
