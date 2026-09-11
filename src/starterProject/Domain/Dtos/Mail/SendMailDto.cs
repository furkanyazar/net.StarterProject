using Core.Application.Dtos;

namespace Domain.Dtos.Mail;

public class SendMailDto : IDto
{
    public string AppDomain { get; set; }
    public string? Locale { get; set; }
    public string AppName { get; set; }

    public SendMailDto()
    {
        AppDomain = string.Empty;
        AppName = string.Empty;
    }

    public SendMailDto(string appDomain, string? locale, string appName)
    {
        AppDomain = appDomain;
        Locale = locale;
        AppName = appName;
    }
}
