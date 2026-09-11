using Application.Features.Auth.Commands.Login;
using Application.Services.AuthService;
using Application.Services.MailQueueService;
using Application.Services.Repositories;
using Application.Services.UserService;
using Application.Tests.Mocks.Configurations;
using Application.Tests.Mocks.FakeDatas;
using Application.Tests.Mocks.Repositories.Auth;
using Core.Security.JWT;
using Core.Test.Application.Constants;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Application.Tests.Features.Auth.Commands.Login;

public class LoginTests : UserMockRepository
{
    private readonly LoginCommand _command;
    private readonly LoginCommand.LoginCommandHandler _handler;
    private readonly LoginCommandValidator _validator;

    public LoginTests(
        UserFakeData userFakeData,
        LoginCommand command,
        LoginCommandValidator validator
    )
        : base(userFakeData)
    {
        IConfiguration configuration = MockConfiguration.GetConfigurationMock();

        Mock<IMailQueueService> mailQueueService = new();
        Mock<IRefreshTokenRepository> refreshTokenRepository = new();
        TokenOptions tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>()!;

        ITokenHelper<int, int, Guid> tokenHelper = new JwtHelper<int, int, Guid>(tokenOptions);
        IUserService userService = new UserManager(MockRepository.Object, mailQueueService.Object);
        IAuthService authService = new AuthManager(
            configuration,
            refreshTokenRepository.Object,
            tokenHelper,
            Mapper
        );

        _command = command;
        _validator = validator;
        _handler = new(BusinessRules, authService, userService);
    }

    [Fact]
    public void UserEmailEmptyShouldReturnError()
    {
        _command.Email = string.Empty;
        ValidationResult result = _validator.Validate(_command);
        ValidationFailure? error = result.Errors.FirstOrDefault(e =>
            e.PropertyName == "Email" && e.ErrorCode == ValidationErrorCodes.NotEmptyValidator
        );
        Assert.Equal(ValidationErrorCodes.NotEmptyValidator, error?.ErrorCode);
    }
}
