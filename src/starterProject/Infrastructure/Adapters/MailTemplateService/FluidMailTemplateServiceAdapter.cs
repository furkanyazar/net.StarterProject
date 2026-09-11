using Application.Services.MailTemplateService;
using Fluid;

namespace Infrastructure.Adapters.MailTemplateService;

public class FluidMailTemplateServiceAdapter : MailTemplateServiceBase
{
    public override async Task<RenderedEmail> RenderAsync(
        string templateName,
        string? locale,
        object? model
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

        string subject = await File.ReadAllTextAsync(subjectPath);
        string htmlBody = await File.ReadAllTextAsync(htmlPath);
        string textBody = await File.ReadAllTextAsync(textPath);

        if (model is not null)
        {
            FluidParser parser = new();

            IFluidTemplate subjectTemplate = parser.Parse(subject);
            IFluidTemplate htmlTemplate = parser.Parse(htmlBody);
            IFluidTemplate textTemplate = parser.Parse(textBody);

            TemplateOptions options = new()
            {
                MemberAccessStrategy = new UnsafeMemberAccessStrategy(),
            };
            options.MemberAccessStrategy.MemberNameStrategy = MemberNameStrategies.SnakeCase;

            TemplateContext context = new(model, options);

            subject = await subjectTemplate.RenderAsync(context);
            htmlBody = await htmlTemplate.RenderAsync(context);
            textBody = await textTemplate.RenderAsync(context);
        }

        return new RenderedEmail(subject, htmlBody, textBody);
    }
}
