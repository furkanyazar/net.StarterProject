using Application.Features.Auth.Rules;
using Application.Services.AuthenticatorService;
using Application.Services.UserService;
using Domain.Dtos.Mail;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest
{
    public string Email { get; set; }
    public SendMailDto SendMailDto { get; set; } = default!;

    public ForgotPasswordCommand()
    {
        Email = string.Empty;
    }

    public ForgotPasswordCommand(string email, SendMailDto sendMailDto)
    {
        Email = email;
        SendMailDto = sendMailDto;
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

            await authenticatorService.SendForgotPasswordMailToUserEmail(
                addedEmailAuthenticator,
                user!,
                request.SendMailDto
            );
        }
    }
}
