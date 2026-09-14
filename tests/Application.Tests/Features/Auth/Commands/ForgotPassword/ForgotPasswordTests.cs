using Application.Features.Auth.Commands.ForgotPassword;
using Application.Features.Auth.Constants;
using Application.Services.AuthenticatorService;
using Application.Services.MailQueueService;
using Application.Services.UserService;
using Application.Tests.Mocks.FakeDatas;
using Application.Tests.Mocks.Repositories.Auth;
using Core.CrossCuttingConcerns.Exception.Types;
using Core.Security.EmailAuthenticator;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace Application.Tests.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordTests : UserMockRepository
{
    private readonly ForgotPasswordCommand _command;
    private readonly ForgotPasswordCommand.ForgotPasswordCommandHandler _handler;
    private readonly ForgotPasswordCommandValidator _validator;

    public ForgotPasswordTests(
        UserFakeData userFakeData,
        EmailAuthenticatorFakeData emailAuthenticatorFakeData,
        ForgotPasswordCommand command,
        ForgotPasswordCommandValidator validator
    )
        : base(userFakeData)
    {
        Mock<IMailQueueService> mailQueueService = new();
        EmailAuthenticatorMockRepository emailAuthenticatorRepository = new(
            emailAuthenticatorFakeData
        );

        IEmailAuthenticatorHelper emailAuthenticatorHelper = new EmailAuthenticatorHelper();
        IUserService userService = new UserManager(MockRepository.Object, mailQueueService.Object);
        IAuthenticatorService authenticatorService = new AuthenticatorManager(
            emailAuthenticatorHelper,
            emailAuthenticatorRepository.MockRepository.Object,
            mailQueueService.Object
        );

        _command = command;
        _validator = validator;
        _handler = new(BusinessRules, userService, authenticatorService);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void Validate_WhenEmailIsEmpty_ShouldHaveValidationError(string email)
    {
        _command.Email = email;
        TestValidationResult<ForgotPasswordCommand> result = _validator.TestValidate(_command);
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorCode(ErrorCodes.EmailRequired);
    }

    [Theory]
    [InlineData("test")]
    [InlineData("test@")]
    public void Validate_WhenEmailIsNotCorrectType_ShouldHaveValidationError(string email)
    {
        _command.Email = email;
        TestValidationResult<ForgotPasswordCommand> result = _validator.TestValidate(_command);
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorCode(ErrorCodes.EmailType);
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveAnyValidationError()
    {
        _command.Email = "test@mail.com";
        TestValidationResult<ForgotPasswordCommand> result = _validator.TestValidate(_command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldThrowNotFoundException()
    {
        _command.Email = "test@mail";
        async Task Act() => await _handler.Handle(_command, CancellationToken.None);
        await Assert.ThrowsAsync<NotFoundException>(Act);
    }

    [Fact]
    public async Task Handle_WhenCredentialsAreValid_ShouldNotThrowException()
    {
        _command.Email = "test@mail.com";
        _command.SendMailDto = new()
        {
            Locale = "en",
            AppDomain = "http://localhost:3000",
            AppName = "Local Host",
        };
        await _handler.Handle(_command, CancellationToken.None);
    }
}
