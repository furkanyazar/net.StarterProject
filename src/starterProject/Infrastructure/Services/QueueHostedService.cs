using Application.Services.MailTemplateService;
using Application.Services.QueueService;
using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.Mailing;
using Domain.Constants;
using Domain.Dtos.Mail;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MimeKit;

namespace Infrastructure.Services;

public class QueueHostedService(
    QueueServiceBase queueService,
    IMailService mailService,
    ILogger logger,
    IServiceProvider serviceProvider
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await InitializeQueuesAsync();
                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception)
            {
                logger.Error(
                    "An error occurred in QueueBackgroundService. Retrying initialization in 5 seconds..."
                );
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task InitializeQueuesAsync()
    {
        await queueService.DeclareQueueAsync(QueueNames.SendEmailQueue);

        await queueService.BindConsumerAsync(
            queueName: QueueNames.SendEmailQueue,
            handleMessageAsync: (Func<MailDto, Task>)SendEmail,
            enableOrdering: true
        );
    }

    private async Task SendEmail(MailDto mailDto)
    {
        using var scope = serviceProvider.CreateAsyncScope();

        MailTemplateServiceBase mailTemplateService =
            scope.ServiceProvider.GetRequiredService<MailTemplateServiceBase>();

        RenderedEmail email = await mailTemplateService.RenderAsync(
            mailDto.TemplateName,
            mailDto.Locale,
            mailDto.Model
        );

        List<MailboxAddress> toList =
        [
            .. mailDto.ToList.Select(t => new MailboxAddress(t.Name, t.Email)),
        ];
        Mail mail = new()
        {
            Subject = email.Subject,
            HtmlBody = email.HtmlBody,
            TextBody = email.TextBody,
            ToList = toList,
        };

        await mailService.SendEmailAsync(mail);

        await scope.DisposeAsync();
    }
}
