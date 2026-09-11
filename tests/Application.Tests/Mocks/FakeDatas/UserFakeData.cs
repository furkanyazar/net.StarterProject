using Core.Security.Enums;
using Core.Security.Hashing;
using Core.Test.Application.FakeData;
using Domain.Entities;

namespace Application.Tests.Mocks.FakeDatas;

public class UserFakeData : BaseFakeData<User, int>
{
    public override List<User> CreateFakeData()
    {
        HashingHelper.CreatePasswordHash(
            "Passw0rd!",
            out byte[] passwordHash,
            out byte[] passwordSalt
        );

        List<User> users =
        [
            new()
            {
                Id = 1,
                Email = "test@mail.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                AuthenticatorType = AuthenticatorType.None,
                UserGroupId = 0,
                CreatedDate = DateTime.UtcNow,
            },
        ];

        return users;
    }
}
