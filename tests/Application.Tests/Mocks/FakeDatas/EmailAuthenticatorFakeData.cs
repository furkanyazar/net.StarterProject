using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Rules;
using Core.Localization.Abstraction;
using Core.Test.Application.FakeData;
using Domain.Entities;

namespace Application.Tests.Mocks.FakeDatas;

public class EmailAuthenticatorFakeData : BaseFakeData<EmailAuthenticator, Guid>
{
    public override List<EmailAuthenticator> CreateFakeData()
    {
        List<EmailAuthenticator> emailAuthenticators =
        [
            new()
            {
                Id = Guid.NewGuid(),
                UserId = 1,
                ActivationKey = "test-active-key",
                IsVerified = false,
                ExpirationDate = DateTime.UtcNow.AddMinutes(30),
                CreatedDate = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = 1,
                ActivationKey = "test-not-active-key",
                IsVerified = true,
                ExpirationDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = 2,
                ActivationKey = "test-not-exist-user-key",
                IsVerified = false,
                ExpirationDate = DateTime.UtcNow.AddMinutes(30),
                CreatedDate = DateTime.UtcNow,
            },
        ];

        return emailAuthenticators;
    }
}

public class EmailAuthenticatorMappingProfiles : Profile
{
    public EmailAuthenticatorMappingProfiles() { }
}

public class EmailAuthenticatorBusinessRules : BaseBusinessRules
{
    public EmailAuthenticatorBusinessRules(
        ILocalizationService localizationService,
        IEmailAuthenticatorRepository emailAuthenticatorRepository
    ) { }
}
