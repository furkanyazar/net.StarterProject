using Core.Security.JWT;
using Domain.Entities;

namespace Application.Services.AuthService;

public interface IAuthService
{
    public Task<AccessToken> CreateAccessToken(User user);
    public Task<RefreshToken> CreateRefreshToken(User user, string ipAddress);
    public Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken);
    public Task DeleteOldRefreshTokens(int userId);
    public Task<RefreshToken?> GetRefreshTokenByToken(string token);
    public Task RevokeDescendantRefreshTokens(
        RefreshToken refreshToken,
        string ipAddress,
        string reason
    );
    public Task<RefreshToken> RotateRefreshToken(
        User user,
        RefreshToken refreshToken,
        string ipAddress
    );
    public Task RevokeRefreshToken(
        RefreshToken refreshToken,
        string ipAddress,
        string? reason = null,
        string? replacedByToken = null
    );
}
