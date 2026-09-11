using System.Text;
using System.Web;
using Application.Services.MailQueueService;
using Application.Services.MailTemplateService;
using Application.Services.Repositories;
using Core.Mailing;
using Core.Security.EmailAuthenticator;
using Domain.Entities;
using MimeKit;

namespace Application.Services.AuthenticatorService;

public class AuthenticatorManager(
    IEmailAuthenticatorHelper emailAuthenticatorHelper,
    IEmailAuthenticatorRepository emailAuthenticatorRepository,
    IMailQueueService mailQueueService,
    MailTemplateServiceBase mailTemplateService
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

        var model = new
        {
            user.Name,
            ResetLink = url,
            AppName = appName,
            AppDomain = appDomain,
        };
        RenderedEmail email = await mailTemplateService.RenderAsync(
            "ForgotPassword",
            locale,
            model
        );

        List<MailboxAddress> toList = [new(user.Name, user.Email)];
        Mail mail = new()
        {
            Subject = email.Subject,
            HtmlBody = email.HtmlBody,
            TextBody = email.TextBody,
            ToList = toList,
        };

        await mailQueueService.SendAsync(mail);
    }
}
