using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using Application.Services.UserService;
using Core.Security.JWT;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<LoggedResponse>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string IpAddress { get; set; }

    public LoginCommand()
    {
        Email = string.Empty;
        Password = string.Empty;
        IpAddress = string.Empty;
    }

    public LoginCommand(string email, string password, string ipAddress)
    {
        Email = email;
        Password = password;
        IpAddress = ipAddress;
    }

    public class LoginCommandHandler(
        AuthBusinessRules authBusinessRules,
        IAuthService authService,
        IUserService userService
    ) : IRequestHandler<LoginCommand, LoggedResponse>
    {
        public async Task<LoggedResponse> Handle(
            LoginCommand request,
            CancellationToken cancellationToken
        )
        {
            User? user = await userService.GetByEmailAsync(request.Email);
            await authBusinessRules.UserShouldExistWhenLogin(user);
            await authBusinessRules.UserPasswordShouldMatchWhenLogin(
                request.Password,
                user!.PasswordHash,
                user.PasswordSalt
            );

            AccessToken createdAccessToken = await authService.CreateAccessToken(user);

            Domain.Entities.RefreshToken createdRefreshToken = await authService.CreateRefreshToken(
                user,
                request.IpAddress
            );
            Domain.Entities.RefreshToken addedRefreshToken = await authService.AddRefreshToken(
                createdRefreshToken
            );
            await authService.DeleteOldRefreshTokens(user.Id);

            LoggedResponse response = new()
            {
                AccessToken = createdAccessToken,
                RefreshToken = addedRefreshToken,
            };
            return response;
        }
    }
}
