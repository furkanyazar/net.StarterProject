using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Constants;
using Application.Services.AuthService;
using Application.Services.MailQueueService;
using Application.Services.Repositories;
using Application.Services.UserService;
using Application.Tests.Mocks.Configurations;
using Application.Tests.Mocks.FakeDatas;
using Application.Tests.Mocks.Repositories.Auth;
using Core.CrossCuttingConcerns.Exception.Types;
using Core.Security.JWT;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Application.Tests.Features.Auth.Commands.Register;

public class RegisterTests : UserMockRepository
{
    private readonly RegisterCommand _command;
    private readonly RegisterCommand.RegisterCommandHandler _handler;
    private readonly RegisterCommandValidator _validator;

    public RegisterTests(
        UserFakeData userFakeData,
        RegisterCommand command,
        RegisterCommandValidator validator
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
        _handler = new(BusinessRules, authService, userService, Mapper);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void Validate_WhenEmailIsEmpty_ShouldHaveValidationError(string email)
    {
        _command.Email = email;
        TestValidationResult<RegisterCommand> result = _validator.TestValidate(_command);
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorCode(ErrorCodes.EmailRequired);
    }

    [Theory]
    [InlineData("test")]
    [InlineData("test@")]
    public void Validate_WhenEmailIsNotCorrectType_ShouldHaveValidationError(string email)
    {
        _command.Email = email;
        TestValidationResult<RegisterCommand> result = _validator.TestValidate(_command);
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorCode(ErrorCodes.EmailType);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void Validate_WhenPasswordIsEmpty_ShouldHaveValidationError(string password)
    {
        _command.Password = password;
        TestValidationResult<RegisterCommand> result = _validator.TestValidate(_command);
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
        TestValidationResult<RegisterCommand> result = _validator.TestValidate(_command);
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
        TestValidationResult<RegisterCommand> result = _validator.TestValidate(_command);
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
        TestValidationResult<RegisterCommand> result = _validator.TestValidate(_command);
        result
            .ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorCode(ErrorCodes.PasswordAtLeastDigit);
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyValidationError()
    {
        _command.Email = "test@mail.com";
        _command.Password = "Passw0rd!";
        TestValidationResult<RegisterCommand> result = _validator.TestValidate(_command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Handle_WhenUserAlreadyExists_ShouldThrowNotFoundException()
    {
        _command.Email = "test@mail.com";
        _command.Password = "Passw0rd!";
        async Task Act() => await _handler.Handle(_command, CancellationToken.None);
        await Assert.ThrowsAsync<BusinessException>(Act);
    }

    [Fact]
    public async Task Handle_WhenCredentialsAreValid_ShouldReturnRegisteredResponse()
    {
        _command.Email = "test1@mail.com";
        _command.Password = "Passw0rd!";
        _command.SendMailDto = new()
        {
            Locale = "en",
            AppDomain = "http://localhost:3000",
            AppName = "Local Host",
        };
        RegisteredResponse result = await _handler.Handle(_command, CancellationToken.None);
        Assert.NotNull(result);
        Assert.NotNull(result.AccessToken);
    }
}
