using Core.Security.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> builder)
    {
        builder.ToTable("UserGroups").HasKey(ug => ug.Id);

        builder.Property(ug => ug.Id).HasColumnName("Id").IsRequired();
        builder.Property(ug => ug.Name).HasColumnName("Name").IsRequired();
        builder.Property(ug => ug.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(ug => ug.UpdatedDate).HasColumnName("UpdatedDate").IsRequired(false);
        builder.Property(ug => ug.DeletedDate).HasColumnName("DeletedDate").IsRequired(false);

        builder.HasQueryFilter(ug => !ug.DeletedDate.HasValue);

        builder.HasMany(ug => ug.Users);
        builder.HasMany(ug => ug.UserGroupOperationClaims);

        builder.HasData(Seeds);

        builder.HasBaseType((string)null!);
    }

    public static int AdminId => 1;
    private static IEnumerable<UserGroup> Seeds
    {
        get { yield return new() { Id = AdminId, Name = GeneralOperationClaims.Admin }; }
    }
}
