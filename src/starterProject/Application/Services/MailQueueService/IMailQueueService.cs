using Core.Mailing;

namespace Application.Services.MailQueueService;

public interface IMailQueueService
{
    public Task SendAsync(Mail mail);
}
