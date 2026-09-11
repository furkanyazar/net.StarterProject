using Application.Services.QueueService;
using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.Mailing;
using Domain.Constants;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services;

public class QueueHostedService(
    QueueServiceBase queueService,
    IMailService mailService,
    ILogger logger
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
            handleMessageAsync: (Func<Mail, Task>)SendEmail,
            enableOrdering: true
        );
    }

    private async Task SendEmail(Mail mail)
    {
        await mailService.SendEmailAsync(mail);
    }
}
