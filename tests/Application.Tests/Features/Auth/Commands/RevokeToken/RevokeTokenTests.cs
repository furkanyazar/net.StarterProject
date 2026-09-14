using Application.Features.Auth.Commands.RevokeToken;
using Application.Services.AuthService;
using Application.Tests.Mocks.Configurations;
using Application.Tests.Mocks.FakeDatas;
using Application.Tests.Mocks.Repositories.Auth;
using Core.CrossCuttingConcerns.Exception.Types;
using Core.Security.JWT;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Application.Tests.Features.Auth.Commands.RevokeToken;

public class RevokeTokenTests : UserMockRepository
{
    private readonly RevokeTokenCommand _command;
    private readonly RevokeTokenCommand.RevokeTokenCommandHandler _handler;

    public RevokeTokenTests(UserFakeData userFakeData, RefreshTokenFakeData refreshTokenFakeData, RevokeTokenCommand command)
        : base(userFakeData)
    {
        IConfiguration configuration = MockConfiguration.GetConfigurationMock();

        TokenOptions tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>()!;
        RefreshTokenMockRepository refreshTokenRepository = new(refreshTokenFakeData);

        ITokenHelper<int, int, Guid> tokenHelper = new JwtHelper<int, int, Guid>(tokenOptions);
        IAuthService authService = new AuthManager(
            configuration,
            refreshTokenRepository.MockRepository.Object,
            tokenHelper,
            Mapper
        );

        _command = command;
        _handler = new(BusinessRules, authService, Mapper);
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenDoesNotExist_ShouldThrowNotFoundException()
    {
        _command.Token = "test";
        async Task Act() => await _handler.Handle(_command, CancellationToken.None);
        await Assert.ThrowsAsync<NotFoundException>(Act);
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenDoesNotActive_ShouldThrowBusinessException()
    {
        _command.Token = "test-not-active-token";
        async Task Act() => await _handler.Handle(_command, CancellationToken.None);
        await Assert.ThrowsAsync<BusinessException>(Act);
    }

    [Fact]
    public async Task Handle_WhenCredentialsAreValid_ShouldReturnRevokedTokenResponse()
    {
        _command.Token = "test-active-token";
        RevokedTokenResponse result = await _handler.Handle(_command, CancellationToken.None);
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
    }
}
