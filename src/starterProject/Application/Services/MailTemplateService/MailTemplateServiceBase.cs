namespace Application.Services.MailTemplateService;

public abstract class MailTemplateServiceBase
{
    public abstract Task<RenderedEmail> RenderAsync(
        string templateName,
        string? locale,
        object? model
    );
}

public sealed record RenderedEmail(string Subject, string HtmlBody, string TextBody);
