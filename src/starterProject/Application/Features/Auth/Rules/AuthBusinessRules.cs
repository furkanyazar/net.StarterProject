using Application.Features.Auth.Constants;
using Application.Services.Repositories;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exception.Types;
using Core.Localization.Abstraction;
using Core.Security.Hashing;
using Domain.Entities;

namespace Application.Features.Auth.Rules;

public class AuthBusinessRules(
    ILocalizationService localizationService,
    IUserRepository userRepository
) : BaseBusinessRules
{
    public async Task UserShouldExistWhenLogin(User? user)
    {
        if (user is null)
            throw new NotFoundException(
                await localizationService.GetLocalizedAsync(AuthMessages.EmailOrPasswordIncorrect)
            );
    }

    public async Task UserShouldExistWhenRequested(User? user)
    {
        if (user is null)
            throw new NotFoundException(
                await localizationService.GetLocalizedAsync(AuthMessages.UserDoesNotExist)
            );
    }

    public async Task UserPasswordShouldMatchWhenLogin(
        string password,
        byte[] passwordHash,
        byte[] passwordSalt
    )
    {
        if (!HashingHelper.VerifyPasswordHash(password, passwordHash, passwordSalt))
            throw new BusinessException(
                await localizationService.GetLocalizedAsync(AuthMessages.EmailOrPasswordIncorrect)
            );
    }

    public async Task EmailCanNotBeDuplicatedWhenRegistered(string email)
    {
        bool userExists = await userRepository.AnyAsync(u => u.Email == email);
        if (userExists)
            throw new BusinessException(
                await localizationService.GetLocalizedAsync(AuthMessages.EmailAlreadyExists)
            );
    }

    public async Task RefreshTokenShouldExistWhenSelected(RefreshToken? refreshToken)
    {
        if (refreshToken is null)
            throw new NotFoundException(
                await localizationService.GetLocalizedAsync(AuthMessages.RefreshTokenDoesNotExist)
            );
    }

    public async Task RefreshTokenShouldBeActiveWhenSelected(RefreshToken refreshToken)
    {
        if (refreshToken.RevokedDate != null || DateTime.UtcNow >= refreshToken.ExpirationDate)
            throw new BusinessException(
                await localizationService.GetLocalizedAsync(AuthMessages.RefreshTokenDoesNotActive)
            );
    }

    public async Task EmailAuthenticatorShouldExistWhenSelected(
        EmailAuthenticator? emailAuthenticator
    )
    {
        if (emailAuthenticator is null)
            throw new NotFoundException(
                await localizationService.GetLocalizedAsync(
                    AuthMessages.EmailAuthenticatorDoesNotExist
                )
            );
    }

    public async Task EmailAuthenticatorShouldBeActiveWhenSelected(
        EmailAuthenticator emailAuthenticator
    )
    {
        if (DateTime.UtcNow >= emailAuthenticator.ExpirationDate || emailAuthenticator.IsVerified)
            throw new BusinessException(
                await localizationService.GetLocalizedAsync(
                    AuthMessages.EmailAuthenticatorDoesNotActive
                )
            );
    }
}
