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
        List<EmailAuthenticator> emailAuthenticators = [];

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
