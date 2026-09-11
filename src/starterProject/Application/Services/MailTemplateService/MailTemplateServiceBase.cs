namespace Application.Services.MailTemplateService;

public abstract class MailTemplateServiceBase
{
    public abstract Task<RenderedEmail> RenderAsync<TModel>(
        string templateName,
        string? locale,
        TModel model
    );
}

public sealed record RenderedEmail(string Subject, string HtmlBody, string TextBody);
