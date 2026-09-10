using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using AutoMapper;
using MediatR;

namespace Application.Features.Auth.Commands.RevokeToken;

public class RevokeTokenCommand : IRequest<RevokedTokenResponse>
{
    public string Token { get; set; }
    public string IpAddress { get; set; }

    public RevokeTokenCommand()
    {
        Token = string.Empty;
        IpAddress = string.Empty;
    }

    public RevokeTokenCommand(string token, string ipAddress)
    {
        Token = token;
        IpAddress = ipAddress;
    }

    public class RevokeTokenCommandHandler(
        AuthBusinessRules authBusinessRules,
        IAuthService authService,
        IMapper mapper
    ) : IRequestHandler<RevokeTokenCommand, RevokedTokenResponse>
    {
        public async Task<RevokedTokenResponse> Handle(
            RevokeTokenCommand request,
            CancellationToken cancellationToken
        )
        {
            Domain.Entities.RefreshToken? refreshToken = await authService.GetRefreshTokenByToken(
                request.Token
            );
            await authBusinessRules.RefreshTokenShouldExistWhenSelected(refreshToken);
            await authBusinessRules.RefreshTokenShouldBeActiveWhenSelected(refreshToken!);

            await authService.RevokeRefreshToken(
                refreshToken: refreshToken!,
                request.IpAddress,
                reason: "Revoked without replacement"
            );

            RevokedTokenResponse revokedTokenResponse = mapper.Map<RevokedTokenResponse>(
                refreshToken
            );
            return revokedTokenResponse;
        }
    }
}
