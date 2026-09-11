using Application.Services.QueueService;
using Domain.Constants;
using Domain.Dtos.Mail;

namespace Application.Services.MailQueueService;

public class MailQueueManager(QueueServiceBase queueService) : IMailQueueService
{
    public async Task SendAsync(MailDto mailDto)
    {
        await queueService.PublishAsync(QueueNames.SendEmailQueue, mailDto);
    }
}
