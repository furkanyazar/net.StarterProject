using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.ResetPassword;
using Application.Services.AuthService;
using Application.Services.MailQueueService;
using Application.Services.Repositories;
using Application.Services.UserService;
using Application.Tests.Mocks.Configurations;
using Application.Tests.Mocks.FakeDatas;
using Application.Tests.Mocks.Repositories.Auth;
using Core.CrossCuttingConcerns.Exception.Types;
using Core.Security.JWT;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Application.Tests.Features.Auth.Commands.RefreshToken;

public class RefreshTokenTests : UserMockRepository
{
    private readonly RefreshTokenCommand _command;
    private readonly RefreshTokenCommand.RefreshTokenCommandHandler _handler;

    public RefreshTokenTests(UserFakeData userFakeData, RefreshTokenFakeData refreshTokenFakeData, RefreshTokenCommand command)
        : base(userFakeData)
    {
        IConfiguration configuration = MockConfiguration.GetConfigurationMock();

        Mock<IMailQueueService> mailQueueService = new();
        TokenOptions tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>()!;
        RefreshTokenMockRepository refreshTokenRepository = new RefreshTokenMockRepository(refreshTokenFakeData);

        ITokenHelper<int, int, Guid> tokenHelper = new JwtHelper<int, int, Guid>(tokenOptions);
        IUserService userService = new UserManager(MockRepository.Object, mailQueueService.Object);
        IAuthService authService = new AuthManager(
            configuration,
            refreshTokenRepository.MockRepository.Object,
            tokenHelper,
            Mapper
        );

        _command = command;
        _handler = new(BusinessRules, authService, userService);
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenDoesNotExist_ShouldThrowNotFoundException()
    {
        _command.RefreshToken = "test";
        async Task Act() => await _handler.Handle(_command, CancellationToken.None);
        await Assert.ThrowsAsync<NotFoundException>(Act);
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenDoesNotActive_ShouldThrowBusinessException()
    {
        _command.RefreshToken = "test-not-active-token";
        async Task Act() => await _handler.Handle(_command, CancellationToken.None);
        await Assert.ThrowsAsync<BusinessException>(Act);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldThrowNotFoundException()
    {
        _command.RefreshToken = "test-not-exist-user-token";
        async Task Act() => await _handler.Handle(_command, CancellationToken.None);
        await Assert.ThrowsAsync<NotFoundException>(Act);
    }

    [Fact]
    public async Task Handle_WhenCredentialsAreValid_ShouldReturnRefreshedTokenResponse()
    {
        _command.RefreshToken = "test-active-token";
        RefreshedTokenResponse result = await _handler.Handle(_command, CancellationToken.None);
        Assert.NotNull(result);
        Assert.NotNull(result.AccessToken);
    }
}
