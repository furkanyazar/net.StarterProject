using Core.Security.Enums;
using Core.Security.Hashing;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users").HasKey(u => u.Id);

        builder.Property(u => u.Id).HasColumnName("Id").IsRequired();
        builder.Property(u => u.Email).HasColumnName("Email").IsRequired();
        builder.Property(u => u.PasswordSalt).HasColumnName("PasswordSalt").IsRequired();
        builder.Property(u => u.PasswordHash).HasColumnName("PasswordHash").IsRequired();
        builder.Property(u => u.AuthenticatorType).HasColumnName("AuthenticatorType").IsRequired();
        builder.Property(u => u.UserGroupId).HasColumnName("UserGroupId").IsRequired();
        builder.Property(u => u.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(u => u.UpdatedDate).HasColumnName("UpdatedDate").IsRequired(false);
        builder.Property(u => u.DeletedDate).HasColumnName("DeletedDate").IsRequired(false);

        builder.Ignore(u => u.Name);

        builder.HasQueryFilter(u => !u.DeletedDate.HasValue);

        builder.HasOne(u => u.UserGroup);
        builder.HasMany(u => u.RefreshTokens);
        builder.HasMany(u => u.EmailAuthenticators);

        builder.HasData(Seeds);

        builder.HasBaseType((string)null!);
    }

    private static IEnumerable<User> Seeds
    {
        get
        {
            HashingHelper.CreatePasswordHash(
                "Passw0rd!",
                out byte[] passwordHash,
                out byte[] passwordSalt
            );
            User adminUser = new()
            {
                Id = 1,
                Email = "test@mail.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                AuthenticatorType = AuthenticatorType.None,
                UserGroupId = UserGroupConfiguration.AdminId,
            };
            yield return adminUser;
        }
    }
}
