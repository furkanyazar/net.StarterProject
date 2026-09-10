namespace Domain.Entities;

public class OperationClaim : Core.Security.Entities.OperationClaim<int>
{
    public virtual ICollection<UserGroupOperationClaim> UserGroupOperationClaims { get; set; } =
        default!;
}
