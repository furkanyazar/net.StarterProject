using Application.Services.Repositories;
using Application.Tests.Mocks.FakeDatas;
using Core.Test.Application.Repositories;
using Domain.Entities;

namespace Application.Tests.Mocks.Repositories.Auth;

public class EmailAuthenticatorMockRepository(EmailAuthenticatorFakeData emailAuthenticatorFakeData)
    : BaseMockRepository<
        IEmailAuthenticatorRepository,
        EmailAuthenticator,
        Guid,
        EmailAuthenticatorMappingProfiles,
        EmailAuthenticatorBusinessRules,
        EmailAuthenticatorFakeData
    >(emailAuthenticatorFakeData) { }
