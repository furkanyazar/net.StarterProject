namespace Domain.Entities;

public class User : Core.Security.Entities.User<int>
{
    public int UserGroupId { get; set; }

    public virtual UserGroup UserGroup { get; set; } = default!;
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = default!;
    public virtual ICollection<EmailAuthenticator> EmailAuthenticators { get; set; } = default!;
}
