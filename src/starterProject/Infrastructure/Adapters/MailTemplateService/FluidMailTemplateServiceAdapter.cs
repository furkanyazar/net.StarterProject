using Application.Services.MailTemplateService;
using Fluid;

namespace Infrastructure.Adapters.MailTemplateService;

public class FluidMailTemplateServiceAdapter : MailTemplateServiceBase
{
    public override async Task<RenderedEmail> RenderAsync<TModel>(
        string templateName,
        string? locale,
        TModel model
    )
    {
        string folderPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "Dropbox",
            "MailTemplates",
            templateName,
            locale ?? "en"
        );

        string subjectPath = Path.Combine(folderPath, "Subject.txt");
        string htmlPath = Path.Combine(folderPath, "Email.html");
        string textPath = Path.Combine(folderPath, "Email.txt");

        bool subjectExist = File.Exists(subjectPath);
        bool htmlExist = File.Exists(htmlPath);
        bool textExist = File.Exists(textPath);

        if (!subjectExist || !htmlExist || !textExist)
            throw new FileNotFoundException();

        string subjectContent = await File.ReadAllTextAsync(subjectPath);
        string htmlContent = await File.ReadAllTextAsync(htmlPath);
        string textContent = await File.ReadAllTextAsync(textPath);

        FluidParser parser = new();

        IFluidTemplate subjectTemplate = parser.Parse(subjectContent);
        IFluidTemplate htmlTemplate = parser.Parse(htmlContent);
        IFluidTemplate textTemplate = parser.Parse(textContent);

        TemplateOptions options = new() { MemberAccessStrategy = new UnsafeMemberAccessStrategy() };
        options.MemberAccessStrategy.MemberNameStrategy = MemberNameStrategies.SnakeCase;

        TemplateContext context = new(model, options);

        string subject = await subjectTemplate.RenderAsync(context);
        string htmlBody = await htmlTemplate.RenderAsync(context);
        string textBody = await textTemplate.RenderAsync(context);

        return new RenderedEmail(subject, htmlBody, textBody);
    }
}
