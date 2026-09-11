using Application.Services.Repositories;
using AutoMapper;
using Core.Security.Entities;
using Core.Security.JWT;
using Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Application.Services.AuthService;

public class AuthManager : IAuthService
{
    private readonly TokenOptions _tokenOptions;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenHelper<int, int, Guid> _tokenHelper;
    private readonly IMapper _mapper;

    public AuthManager(
        IConfiguration configuration,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenHelper<int, int, Guid> tokenHelper,
        IMapper mapper
    )
    {
        const string tokenOptionsConfigurationSection = "TokenOptions";
        _tokenOptions =
            configuration.GetSection(tokenOptionsConfigurationSection).Get<TokenOptions>()
            ?? throw new NullReferenceException(
                $"\"{tokenOptionsConfigurationSection}\" section cannot found in configuration"
            );

        _refreshTokenRepository = refreshTokenRepository;
        _tokenHelper = tokenHelper;
        _mapper = mapper;
    }

    public async Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken)
    {
        RefreshToken addedRefreshToken = await _refreshTokenRepository.AddAsync(refreshToken);
        return addedRefreshToken;
    }

    public Task<AccessToken> CreateAccessToken(User user)
    {
        AccessToken accessToken = _tokenHelper.CreateToken(user, []);
        return Task.FromResult(accessToken);
    }

    public Task<RefreshToken> CreateRefreshToken(User user, string ipAddress)
    {
        RefreshToken<Guid, int> coreRefreshToken = _tokenHelper.CreateRefreshToken(user, ipAddress);
        RefreshToken refreshToken = _mapper.Map<RefreshToken>(coreRefreshToken);
        return Task.FromResult(refreshToken);
    }

    public async Task DeleteOldRefreshTokens(int userId)
    {
        ICollection<RefreshToken> refreshTokens = await _refreshTokenRepository.GetAllAsync(
            predicate: rt =>
                rt.UserId == userId
                && rt.RevokedDate == null
                && rt.ExpirationDate >= DateTime.UtcNow
                && rt.CreatedDate.AddDays(_tokenOptions.RefreshTokenTTL) <= DateTime.UtcNow,
            enableTracking: false
        );
        await _refreshTokenRepository.DeleteRangeAsync(refreshTokens);
    }

    public async Task<RefreshToken?> GetRefreshTokenByToken(string token)
    {
        RefreshToken? refreshToken = await _refreshTokenRepository.GetAsync(predicate: rt =>
            rt.Token == token
        );
        return refreshToken;
    }

    public async Task RevokeDescendantRefreshTokens(
        RefreshToken refreshToken,
        string ipAddress,
        string reason
    )
    {
        RefreshToken? childToken = await _refreshTokenRepository.GetAsync(predicate: r =>
            r.Token == refreshToken.ReplacedByToken
        );

        if (childToken?.RevokedDate != null && childToken.ExpirationDate <= DateTime.UtcNow)
            await RevokeRefreshToken(childToken, ipAddress, reason);
        else
            await RevokeDescendantRefreshTokens(refreshToken: childToken!, ipAddress, reason);
    }

    public async Task RevokeRefreshToken(
        RefreshToken refreshToken,
        string ipAddress,
        string? reason = null,
        string? replacedByToken = null
    )
    {
        refreshToken.RevokedDate = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReasonRevoked = reason;
        refreshToken.ReplacedByToken = replacedByToken;
        await _refreshTokenRepository.UpdateAsync(refreshToken);
    }

    public async Task<RefreshToken> RotateRefreshToken(
        User user,
        RefreshToken refreshToken,
        string ipAddress
    )
    {
        RefreshToken<Guid, int> newCoreRefreshToken = _tokenHelper.CreateRefreshToken(
            user,
            ipAddress
        );
        RefreshToken newRefreshToken = _mapper.Map<RefreshToken>(newCoreRefreshToken);
        await RevokeRefreshToken(
            refreshToken,
            ipAddress,
            reason: "Replaced by new token",
            newRefreshToken.Token
        );
        return newRefreshToken;
    }
}
