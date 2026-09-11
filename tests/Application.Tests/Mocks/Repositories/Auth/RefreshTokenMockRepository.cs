using Application.Features.Auth.Profiles;
using Application.Features.Auth.Rules;
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
        MappingProfiles,
        AuthBusinessRules,
        RefreshTokenFakeData
    >(refreshTokenFakeData) { }
