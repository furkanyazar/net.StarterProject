using Application.Services.MailQueueService;
using Domain.Entities;

namespace Application.Services.AuthenticatorService;

public interface IAuthenticatorService
{
    public Task<EmailAuthenticator> AddEmailAuthenticator(EmailAuthenticator emailAuthenticator);
    public Task<EmailAuthenticator> CreateEmailAuthenticator(User user);
    public Task SendForgotPasswordToUserEmail(
        EmailAuthenticator emailAuthenticator,
        User user,
        string appDomain,
        string? locale,
        string appName
    );
}
