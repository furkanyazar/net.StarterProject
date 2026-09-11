using AutoMapper;
using Core.Application.Rules;
using Core.Test.Application.FakeData;
using Domain.Entities;

namespace Application.Tests.Mocks.FakeDatas;

public class RefreshTokenFakeData : BaseFakeData<RefreshToken, Guid>
{
    public override List<RefreshToken> CreateFakeData()
    {
        List<RefreshToken> refreshTokens = [];

        return refreshTokens;
    }
}

public class RefreshTokenFakeMappingProfiles : Profile { }

public class RefreshTokenFakeBusinessRules : BaseBusinessRules { }
