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
            ExpirationDate = DateTime.UtcNow.AddMinutes(30),
        };
        return emailAuthenticator;
    }

    public async Task<EmailAuthenticator?> GetEmailAuthenticatorByActivationKey(
        string activationKey
    )
    {
        EmailAuthenticator? emailAuthenticator = await emailAuthenticatorRepository.GetAsync(ea =>
            ea.ActivationKey == activationKey
        );
        return emailAuthenticator;
    }

    public async Task SendForgotPasswordMailToUserEmail(
        EmailAuthenticator emailAuthenticator,
        User user,
        SendMailDto sendMailDto
    )
    {
        string key = HttpUtility.UrlEncode(emailAuthenticator.ActivationKey!, Encoding.UTF8);
        string url = $"{sendMailDto.AppDomain}/reset-password/{key}";

        MailDto mailDto = new()
        {
            TemplateName = "ForgotPassword",
            Locale = sendMailDto.Locale,
            ToList = [new(user.Name, user.Email)],
            Model = new
            {
                user.Name,
                sendMailDto.AppName,
                sendMailDto.AppDomain,
                ResetLink = url,
            },
        };

        await mailQueueService.SendAsync(mailDto);
    }

    public async Task VerifyEmailAuthenticator(EmailAuthenticator emailAuthenticator)
    {
        emailAuthenticator.IsVerified = true;
        await emailAuthenticatorRepository.UpdateAsync(emailAuthenticator);
    }
}
