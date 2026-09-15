namespace Domain.Entities;

public class UserGroupOperationClaim
    : Core.Security.Entities.UserGroupOperationClaim<Guid, int, int>
{
    public virtual UserGroup UserGroup { get; set; } = default!;
    public virtual OperationClaim OperationClaim { get; set; } = default!;
}
