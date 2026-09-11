using System.Text;
using System.Web;
using Application.Services.MailQueueService;
using Application.Services.Repositories;
using Core.Security.EmailAuthenticator;
using Domain.Dtos.Mail;
using Domain.Entities;

namespace Application.Services.AuthenticatorService;

public class AuthenticatorManager(
    IEmailAuthenticatorHelper emailAuthenticatorHelper,
    IEmailAuthenticatorRepository emailAuthenticatorRepository,
    IMailQueueService mailQueueService
) : IAuthenticatorService
{
    public async Task<EmailAuthenticator> AddEmailAuthenticator(
        EmailAuthenticator emailAuthenticator
    )
    {
        EmailAuthenticator createdEmailAuthenticator = await emailAuthenticatorRepository.AddAsync(
            emailAuthenticator
        );
        return createdEmailAuthenticator;
    }

    public async Task<EmailAuthenticator> CreateEmailAuthenticator(User user)
    {
        EmailAuthenticator emailAuthenticator = new()
        {
            UserId = user.Id,
            ActivationKey = await emailAuthenticatorHelper.CreateEmailActivationKey(),
            IsVerified = false,
        };
        return emailAuthenticator;
    }

    public async Task SendForgotPasswordToUserEmail(
        EmailAuthenticator emailAuthenticator,
        User user,
        string appDomain,
        string? locale,
        string appName
    )
    {
        string key = HttpUtility.UrlEncode(emailAuthenticator.ActivationKey!, Encoding.UTF8);
        string url = $"{appDomain}/reset-password/{key}";

        MailDto mailDto = new()
        {
            TemplateName = "ForgotPassword",
            Locale = locale,
            ToList = [new(user.Name, user.Email)],
            Model = new
            {
                user.Name,
                ResetLink = url,
                AppName = appName,
                AppDomain = appDomain,
            },
        };

        await mailQueueService.SendAsync(mailDto);
    }
}
