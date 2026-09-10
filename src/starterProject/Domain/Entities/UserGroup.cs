namespace Domain.Entities;

public class UserGroup : Core.Security.Entities.UserGroup<int>
{
    public virtual ICollection<User> Users { get; set; } = default!;
    public virtual ICollection<UserGroupOperationClaim> UserGroupOperationClaims { get; set; } =
        default!;
}
