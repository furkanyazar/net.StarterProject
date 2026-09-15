using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class UserGroupOperationClaimConfiguration
    : IEntityTypeConfiguration<UserGroupOperationClaim>
{
    public void Configure(EntityTypeBuilder<UserGroupOperationClaim> builder)
    {
        builder.ToTable("UserGroupOperationClaims").HasKey(ugoc => ugoc.Id);

        builder.Property(ugoc => ugoc.Id).HasColumnName("Id").IsRequired();
        builder.Property(ugoc => ugoc.UserGroupId).HasColumnName("UserGroupId").IsRequired();
        builder
            .Property(ugoc => ugoc.OperationClaimId)
            .HasColumnName("OperationClaimId")
            .IsRequired();
        builder.Property(ugoc => ugoc.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(ugoc => ugoc.UpdatedDate).HasColumnName("UpdatedDate").IsRequired(false);
        builder.Property(ugoc => ugoc.DeletedDate).HasColumnName("DeletedDate").IsRequired(false);

        builder.HasQueryFilter(ugoc => !ugoc.DeletedDate.HasValue);

        builder.HasOne(ugoc => ugoc.UserGroup);
        builder.HasOne(ugoc => ugoc.OperationClaim);

        builder.HasData(Seeds);

        builder.HasBaseType((string)null!);
    }

    private static IEnumerable<UserGroupOperationClaim> Seeds
    {
        get
        {
            yield return new()
            {
                Id = Guid.NewGuid(),
                OperationClaimId = OperationClaimConfiguration.AdminId,
                UserGroupId = UserGroupConfiguration.AdminId,
            };
        }
    }
}
