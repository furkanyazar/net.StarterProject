using Core.Application.Dtos;

namespace Domain.Dtos.Mail;

public class MailDto : IDto
{
    public string TemplateName { get; set; }
    public string? Locale { get; set; }
    public List<MailAddressDto> ToList { get; set; }
    public object? Model { get; set; }

    public MailDto()
    {
        TemplateName = string.Empty;
        ToList = [];
    }

    public MailDto(string templateName, string? locale, List<MailAddressDto> toList, object? model)
    {
        TemplateName = templateName;
        Locale = locale;
        ToList = toList;
        Model = model;
    }
}

public class MailAddressDto : IDto
{
    public string? Name { get; set; }
    public string Email { get; set; }

    public MailAddressDto()
    {
        Email = string.Empty;
    }

    public MailAddressDto(string? name, string email)
    {
        Name = name;
        Email = email;
    }
}
