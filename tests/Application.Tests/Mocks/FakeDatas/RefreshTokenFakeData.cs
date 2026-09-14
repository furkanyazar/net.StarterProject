using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Rules;
using Core.Localization.Abstraction;
using Core.Test.Application.FakeData;
using Domain.Entities;

namespace Application.Tests.Mocks.FakeDatas;

public class RefreshTokenFakeData : BaseFakeData<RefreshToken, Guid>
{
    public override List<RefreshToken> CreateFakeData()
    {
        List<RefreshToken> refreshTokens =
        [
            new()
            {
                Id = Guid.NewGuid(),
                UserId = 1,
                Token = "test-active-token",
                ExpirationDate = DateTime.UtcNow.AddDays(7),
                CreatedByIp = string.Empty,
                CreatedDate = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = 1,
                Token = "test-not-active-token",
                ExpirationDate = DateTime.UtcNow.AddDays(-7),
                CreatedByIp = string.Empty,
                CreatedDate = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = 2,
                Token = "test-not-exist-user-token",
                ExpirationDate = DateTime.UtcNow.AddDays(7),
                CreatedByIp = string.Empty,
                CreatedDate = DateTime.UtcNow,
            },
        ];

        return refreshTokens;
    }
}

public class RefreshTokenMappingProfiles : Profile
{
    public RefreshTokenMappingProfiles() { }
}

public class RefreshTokenBusinessRules : BaseBusinessRules
{
    public RefreshTokenBusinessRules(
        ILocalizationService localizationService,
        IRefreshTokenRepository emailAuthenticatorRepository
    ) { }
}
