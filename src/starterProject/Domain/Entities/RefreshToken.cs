namespace Domain.Entities;

public class RefreshToken : Core.Security.Entities.RefreshToken<Guid, int>
{
    public virtual User User { get; set; } = default!;
}
