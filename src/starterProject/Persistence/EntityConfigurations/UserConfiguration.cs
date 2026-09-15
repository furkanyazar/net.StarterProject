using Core.Security.Enums;
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
            User adminUser = new()
            {
                Id = 1,
                Email = "test@mail.com",
                PasswordHash =
                [
                    251,
                    155,
                    223,
                    178,
                    156,
                    188,
                    19,
                    135,
                    241,
                    146,
                    63,
                    80,
                    72,
                    86,
                    233,
                    237,
                    164,
                    225,
                    118,
                    60,
                    55,
                    40,
                    244,
                    157,
                    233,
                    21,
                    203,
                    129,
                    168,
                    240,
                    84,
                    78,
                    90,
                    156,
                    145,
                    126,
                    44,
                    239,
                    246,
                    6,
                    95,
                    56,
                    22,
                    53,
                    36,
                    220,
                    219,
                    108,
                    40,
                    68,
                    4,
                    26,
                    4,
                    240,
                    228,
                    28,
                    39,
                    82,
                    237,
                    95,
                    197,
                    174,
                    141,
                    141,
                ],
                PasswordSalt =
                [
                    254,
                    83,
                    56,
                    11,
                    254,
                    73,
                    157,
                    180,
                    95,
                    203,
                    28,
                    4,
                    79,
                    125,
                    176,
                    235,
                    87,
                    56,
                    14,
                    197,
                    139,
                    131,
                    39,
                    160,
                    122,
                    165,
                    59,
                    250,
                    55,
                    58,
                    135,
                    18,
                    6,
                    129,
                    122,
                    121,
                    154,
                    2,
                    176,
                    59,
                    154,
                    39,
                    125,
                    200,
                    124,
                    22,
                    207,
                    152,
                    130,
                    221,
                    60,
                    95,
                    132,
                    222,
                    13,
                    176,
                    14,
                    35,
                    106,
                    57,
                    224,
                    66,
                    184,
                    239,
                    184,
                    96,
                    225,
                    142,
                    112,
                    178,
                    54,
                    106,
                    48,
                    161,
                    146,
                    100,
                    87,
                    46,
                    5,
                    206,
                    132,
                    131,
                    106,
                    48,
                    119,
                    1,
                    167,
                    50,
                    209,
                    39,
                    13,
                    58,
                    12,
                    117,
                    144,
                    213,
                    50,
                    116,
                    22,
                    167,
                    82,
                    71,
                    163,
                    72,
                    191,
                    6,
                    240,
                    206,
                    96,
                    247,
                    96,
                    189,
                    101,
                    69,
                    38,
                    120,
                    228,
                    181,
                    140,
                    128,
                    87,
                    150,
                    152,
                    208,
                    132,
                    46,
                    10,
                    81,
                ],
                AuthenticatorType = AuthenticatorType.None,
                UserGroupId = UserGroupConfiguration.AdminId,
            };
            yield return adminUser;
        }
    }
}
