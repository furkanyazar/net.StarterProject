using Application.Features.Auth.Profiles;
using Application.Features.Auth.Rules;
using Application.Services.Repositories;
using Application.Tests.Mocks.FakeData;
using Core.Test.Application.Repositories;
using Domain.Entities;

namespace Application.Tests.Mocks.Repositories;

public class UserMockRepository(UserFakeData fakeData)
    : BaseMockRepository<
        IUserRepository,
        User,
        int,
        MappingProfiles,
        AuthBusinessRules,
        UserFakeData
    >(fakeData) { }
