using Domain.Dtos.Mail;
using Domain.Entities;

namespace Application.Services.AuthenticatorService;

public interface IAuthenticatorService
{
    public Task<EmailAuthenticator> AddEmailAuthenticator(EmailAuthenticator emailAuthenticator);
    public Task<EmailAuthenticator> CreateEmailAuthenticator(User user);
    public Task<EmailAuthenticator?> GetEmailAuthenticatorByActivationKey(string activationKey);
    public Task SendForgotPasswordMailToUserEmail(
        EmailAuthenticator emailAuthenticator,
        User user,
        SendMailDto sendMailDto
    );
    public Task VerifyEmailAuthenticator(EmailAuthenticator emailAuthenticator);
}
