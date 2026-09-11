using Application.Services.Repositories;
using Application.Tests.Mocks.FakeDatas;
using Core.Test.Application.Repositories;
using Domain.Entities;

namespace Application.Tests.Mocks.Repositories.Auth;

public class RefreshTokenMockRepository(RefreshTokenFakeData refreshTokenFakeData)
    : BaseMockRepository<
        IRefreshTokenRepository,
        RefreshToken,
        Guid,
        RefreshTokenFakeMappingProfiles,
        RefreshTokenFakeBusinessRules,
        RefreshTokenFakeData
    >(refreshTokenFakeData) { }
