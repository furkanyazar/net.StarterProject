using Application.Features.Auth.Rules;
using Application.Services.AuthenticatorService;
using Application.Services.UserService;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest
{
    public string Email { get; set; }
    public string AppDomain { get; set; }
    public string AppName { get; set; }
    public string? Locale { get; set; }

    public ForgotPasswordCommand()
    {
        Email = string.Empty;
        AppDomain = string.Empty;
        AppName = string.Empty;
    }

    public ForgotPasswordCommand(string email, string appDomain, string appName, string? locale)
    {
        Email = email;
        AppDomain = appDomain;
        AppName = appName;
        Locale = locale;
    }

    public class ForgotPasswordCommandHandler(
        AuthBusinessRules authBusinessRules,
        IUserService userService,
        IAuthenticatorService authenticatorService
    ) : IRequestHandler<ForgotPasswordCommand>
    {
        public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            User? user = await userService.GetByEmail(request.Email);
            await authBusinessRules.UserShouldExistWhenRequested(user);

            EmailAuthenticator createdEmailAuthenticator =
                await authenticatorService.CreateEmailAuthenticator(user!);
            EmailAuthenticator addedEmailAuthenticator =
                await authenticatorService.AddEmailAuthenticator(createdEmailAuthenticator);

            await authenticatorService.SendForgotPasswordToUserEmail(
                addedEmailAuthenticator,
                user!,
                request.AppDomain,
                request.Locale,
                request.AppName
            );
        }
    }
}
