using Application.Features.Auth.Commands.Login;
using Application.Services.AuthService;
using Application.Services.UserService;
using Application.Tests.Mocks.FakeDatas;
using Application.Tests.Mocks.Repositories.Auth;
using Core.Security.JWT;
using Microsoft.Extensions.Configuration;

namespace Application.Tests.Features.Auth.Commands.Login;

public class LoginTests : UserMockRepository
{
    private readonly LoginCommand _command;
    private readonly LoginCommand.LoginCommandHandler _handler;
    private readonly LoginCommandValidator _validator;

    public LoginTests(
        IConfiguration configuration,
        UserFakeData userFakeData,
        RefreshTokenFakeData refreshTokenFakeData,
        LoginCommand command,
        LoginCommandValidator validator
    )
        : base(userFakeData)
    {
        TokenOptions tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>()!;
        RefreshTokenMockRepository refreshTokenMockRepository = new(refreshTokenFakeData);

        ITokenHelper<int, int, Guid> tokenHelper = new JwtHelper<int, int, Guid>(tokenOptions);
        IAuthService authService = new AuthManager(
            configuration,
            refreshTokenMockRepository.MockRepository.Object,
            tokenHelper,
            Mapper
        );
        IUserService userService = new UserManager(MockRepository.Object, null);

        _command = command;
        _validator = validator;
        _handler = new(BusinessRules, authService, userService);
    }
}
