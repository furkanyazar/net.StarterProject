namespace Domain.Entities;

public class EmailAuthenticator : Core.Security.Entities.EmailAuthenticator<Guid, int>
{
    public DateTime ExpirationDate { get; set; }

    public virtual User User { get; set; } = default!;
}
