using Application.Features.Auth.Commands.ResetPassword;
using Application.Features.Auth.Constants;
using Application.Services.AuthenticatorService;
using Application.Services.AuthService;
using Application.Services.MailQueueService;
using Application.Services.Repositories;
using Application.Services.UserService;
using Application.Tests.Mocks.Configurations;
using Application.Tests.Mocks.FakeDatas;
using Application.Tests.Mocks.Repositories.Auth;
using Core.CrossCuttingConcerns.Exception.Types;
using Core.Security.EmailAuthenticator;
using Core.Security.JWT;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Application.Tests.Features.Auth.Commands.ResetPassword;

public class ResetPasswordTests : UserMockRepository
{
    private readonly ResetPasswordCommand _command;
    private readonly ResetPasswordCommand.ResetPasswordCommandHandler _handler;
    private readonly ResetPasswordCommandValidator _validator;

    public ResetPasswordTests(
        UserFakeData userFakeData,
        EmailAuthenticatorFakeData emailAuthenticatorFakeData,
        ResetPasswordCommand command,
        ResetPasswordCommandValidator validator
    )
        : base(userFakeData)
    {
        IConfiguration configuration = MockConfiguration.GetConfigurationMock();

        Mock<IMailQueueService> mailQueueService = new();
        Mock<IRefreshTokenRepository> refreshTokenRepository = new();
        TokenOptions tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>()!;
        EmailAuthenticatorMockRepository emailAuthenticatorRepository = new(
            emailAuthenticatorFakeData
        );

        IEmailAuthenticatorHelper emailAuthenticatorHelper = new EmailAuthenticatorHelper();
        ITokenHelper<int, int, Guid> tokenHelper = new JwtHelper<int, int, Guid>(tokenOptions);
        IUserService userService = new UserManager(MockRepository.Object, mailQueueService.Object);
        IAuthService authService = new AuthManager(
            configuration,
            refreshTokenRepository.Object,
            tokenHelper,
            Mapper
        );
        IAuthenticatorService authenticatorService = new AuthenticatorManager(
            emailAuthenticatorHelper,
            emailAuthenticatorRepository.MockRepository.Object,
            mailQueueService.Object
        );

        _command = command;
        _validator = validator;
        _handler = new(authenticatorService, BusinessRules, userService, authService);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void Validate_WhenPasswordIsEmpty_ShouldHaveValidationError(string password)
    {
        _command.Password = password;
        TestValidationResult<ResetPasswordCommand> result = _validator.TestValidate(_command);
        result
            .ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorCode(ErrorCodes.PasswordRequired);
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("1234123")]
    public void Validate_WhenPasswordLengthIsLessThen8_ShouldHaveValidationError(string password)
    {
        _command.Password = password;
        TestValidationResult<ResetPasswordCommand> result = _validator.TestValidate(_command);
        result
            .ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorCode(ErrorCodes.PasswordMinLength);
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("/*-()")]
    public void Validate_WhenPasswordDoesNotHaveLetter_ShouldHaveValidationError(string password)
    {
        _command.Password = password;
        TestValidationResult<ResetPasswordCommand> result = _validator.TestValidate(_command);
        result
            .ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorCode(ErrorCodes.PasswordAtLeastLetter);
    }

    [Theory]
    [InlineData("test")]
    [InlineData("/*-()")]
    public void Validate_WhenPasswordDoesNotHaveDigit_ShouldHaveValidationError(string password)
    {
        _command.Password = password;
        TestValidationResult<ResetPasswordCommand> result = _validator.TestValidate(_command);
        result
            .ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorCode(ErrorCodes.PasswordAtLeastDigit);
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyValidationError()
    {
        _command.Password = "Passw0rd!";
        TestValidationResult<ResetPasswordCommand> result = _validator.TestValidate(_command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Handle_WhenEmailAuthenticatorDoesNotExist_ShouldThrowNotFoundException()
    {
        _command.ActivationKey = "test";
        async Task Act() => await _handler.Handle(_command, CancellationToken.None);
        await Assert.ThrowsAsync<NotFoundException>(Act);
    }

    [Fact]
    public async Task Handle_WhenEmailAuthenticatorDoesNotActive_ShouldThrowBusinessException()
    {
        _command.ActivationKey = "test-not-active-key";
        async Task Act() => await _handler.Handle(_command, CancellationToken.None);
        await Assert.ThrowsAsync<BusinessException>(Act);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldThrowNotFoundException()
    {
        _command.ActivationKey = "test-not-exist-user-key";
        async Task Act() => await _handler.Handle(_command, CancellationToken.None);
        await Assert.ThrowsAsync<NotFoundException>(Act);
    }

    [Fact]
    public async Task Handle_WhenCredentialsAreValid_ShouldReturnResetPasswordResponse()
    {
        _command.ActivationKey = "test-active-key";
        _command.Password = "Passw0rd!";
        _command.SendMailDto = new()
        {
            Locale = "en",
            AppDomain = "http://localhost:3000",
            AppName = "Local Host",
        };
        ResetPasswordResponse result = await _handler.Handle(_command, CancellationToken.None);
        Assert.NotNull(result);
        Assert.NotNull(result.AccessToken);
    }
}
