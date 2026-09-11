using Application.Services.QueueService;
using Core.Mailing;
using Domain.Constants;

namespace Application.Services.MailQueueService;

public class MailQueueManager(QueueServiceBase queueService) : IMailQueueService
{
    public async Task SendAsync(Mail mail)
    {
        await queueService.PublishAsync(QueueNames.SendEmailQueue, mail);
    }
}
