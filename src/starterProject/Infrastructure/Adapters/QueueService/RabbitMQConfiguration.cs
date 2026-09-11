namespace Infrastructure.Adapters.QueueService;

public class RabbitMQConfiguration
{
    public string HostName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Prefix { get; set; }

    public RabbitMQConfiguration()
    {
        HostName = string.Empty;
        UserName = string.Empty;
        Password = string.Empty;
        Prefix = string.Empty;
    }

    public RabbitMQConfiguration(string hostName, string userName, string password, string prefix)
    {
        HostName = hostName;
        UserName = userName;
        Password = password;
        Prefix = prefix;
    }
}
