using Core.Security.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class OperationClaimConfiguration : IEntityTypeConfiguration<OperationClaim>
{
    public void Configure(EntityTypeBuilder<OperationClaim> builder)
    {
        builder.ToTable("OperationClaims").HasKey(oc => oc.Id);

        builder.Property(oc => oc.Id).HasColumnName("Id").IsRequired();
        builder.Property(oc => oc.Name).HasColumnName("Name").IsRequired();
        builder.Property(oc => oc.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(oc => oc.UpdatedDate).HasColumnName("UpdatedDate").IsRequired(false);
        builder.Property(oc => oc.DeletedDate).HasColumnName("DeletedDate").IsRequired(false);

        builder.HasQueryFilter(oc => !oc.DeletedDate.HasValue);

        builder.HasMany(oc => oc.UserGroupOperationClaims);

        builder.HasData(Seeds);

        builder.HasBaseType((string)null!);
    }

    public static int AdminId => 1;
    private static IEnumerable<OperationClaim> Seeds
    {
        get
        {
            yield return new()
            {
                Id = AdminId,
                Name = GeneralOperationClaims.Admin,
                CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            };
        }
    }
}
