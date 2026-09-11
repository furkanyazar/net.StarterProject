using Application.Features.Auth.Rules;
using Application.Services.AuthenticatorService;
using Application.Services.AuthService;
using Application.Services.UserService;
using Core.Security.Hashing;
using Core.Security.JWT;
using Domain.Dtos.Mail;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest<ResetPasswordResponse>
{
    public string ActivationKey { get; set; }
    public string Password { get; set; }
    public string IpAddress { get; set; }
    public SendMailDto SendMailDto { get; set; } = default!;

    public ResetPasswordCommand()
    {
        ActivationKey = string.Empty;
        Password = string.Empty;
        IpAddress = string.Empty;
    }

    public ResetPasswordCommand(
        string activationKey,
        string password,
        string ipAddress,
        SendMailDto sendMailDto
    )
    {
        ActivationKey = activationKey;
        Password = password;
        IpAddress = ipAddress;
        SendMailDto = sendMailDto;
    }

    public class ResetPasswordCommandHandler(
        IAuthenticatorService authenticatorService,
        AuthBusinessRules authBusinessRules,
        IUserService userService,
        IAuthService authService
    ) : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
    {
        public async Task<ResetPasswordResponse> Handle(
            ResetPasswordCommand request,
            CancellationToken cancellationToken
        )
        {
            EmailAuthenticator? emailAuthenticator =
                await authenticatorService.GetEmailAuthenticatorByActivationKey(
                    request.ActivationKey
                );
            await authBusinessRules.EmailAuthenticatorShouldExistWhenSelected(emailAuthenticator);
            await authBusinessRules.EmailAuthenticatorShouldBeActiveWhenSelected(
                emailAuthenticator!
            );

            await authenticatorService.VerifyEmailAuthenticator(emailAuthenticator!);

            User? user = await userService.GetById(emailAuthenticator!.UserId);
            await authBusinessRules.UserShouldExistWhenRequested(user);

            HashingHelper.CreatePasswordHash(
                request.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt
            );
            user!.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            User updatedUser = await userService.UpdateUser(user);
            await userService.SendResetPasswordMailToUserEmail(updatedUser, request.SendMailDto);

            AccessToken createdAccessToken = await authService.CreateAccessToken(updatedUser);

            Domain.Entities.RefreshToken createdRefreshToken = await authService.CreateRefreshToken(
                updatedUser,
                request.IpAddress
            );
            Domain.Entities.RefreshToken addedRefreshToken = await authService.AddRefreshToken(
                createdRefreshToken
            );
            await authService.DeleteOldRefreshTokens(updatedUser.Id);

            ResetPasswordResponse response = new()
            {
                AccessToken = createdAccessToken,
                RefreshToken = addedRefreshToken,
            };
            return response;
        }
    }
}
