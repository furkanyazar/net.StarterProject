using Core.Security.Enums;
using Core.Test.Application.FakeData;
using Domain.Entities;

namespace Application.Tests.Mocks.FakeData;

public class UserFakeData : BaseFakeData<User, int>
{
    public override List<User> CreateFakeData()
    {
        List<User> users =
        [
            new()
            {
                Id = 1,
                Email = "test@mail.com",
                PasswordHash = [],
                PasswordSalt = [],
                AuthenticatorType = AuthenticatorType.None,
                UserGroupId = 0,
            },
        ];
        return users;
    }
}
