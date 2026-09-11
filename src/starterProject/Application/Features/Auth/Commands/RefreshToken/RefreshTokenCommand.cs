using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using Application.Services.UserService;
using Core.Security.JWT;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<RefreshedTokenResponse>
{
    public string RefreshToken { get; set; }
    public string IpAddress { get; set; }

    public RefreshTokenCommand()
    {
        RefreshToken = string.Empty;
        IpAddress = string.Empty;
    }

    public RefreshTokenCommand(string refreshToken, string ipAddress)
    {
        RefreshToken = refreshToken;
        IpAddress = ipAddress;
    }

    public class RefreshTokenCommandHandler(
        AuthBusinessRules authBusinessRules,
        IAuthService authService,
        IUserService userService
    ) : IRequestHandler<RefreshTokenCommand, RefreshedTokenResponse>
    {
        public async Task<RefreshedTokenResponse> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken
        )
        {
            Domain.Entities.RefreshToken? refreshToken = await authService.GetRefreshTokenByToken(
                request.RefreshToken
            );
            await authBusinessRules.RefreshTokenShouldExistWhenSelected(refreshToken);
            await authBusinessRules.RefreshTokenShouldBeActiveWhenSelected(refreshToken!);

            if (refreshToken!.RevokedDate != null)
                await authService.RevokeDescendantRefreshTokens(
                    refreshToken,
                    request.IpAddress,
                    reason: $"Attempted reuse of revoked ancestor token: {refreshToken.Token}"
                );

            User? user = await userService.GetById(refreshToken.UserId);
            await authBusinessRules.UserShouldExistWhenRequested(user);

            Domain.Entities.RefreshToken newRefreshToken = await authService.RotateRefreshToken(
                user: user!,
                refreshToken,
                request.IpAddress
            );
            Domain.Entities.RefreshToken addedRefreshToken = await authService.AddRefreshToken(
                newRefreshToken
            );
            await authService.DeleteOldRefreshTokens(refreshToken.UserId);

            AccessToken createdAccessToken = await authService.CreateAccessToken(user!);

            RefreshedTokenResponse refreshedTokensResponse = new()
            {
                AccessToken = createdAccessToken,
                RefreshToken = addedRefreshToken,
            };
            return refreshedTokensResponse;
        }
    }
}
