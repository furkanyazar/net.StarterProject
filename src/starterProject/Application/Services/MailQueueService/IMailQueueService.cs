using Domain.Dtos.Mail;

namespace Application.Services.MailQueueService;

public interface IMailQueueService
{
    public Task SendAsync(MailDto mailDto);
}
