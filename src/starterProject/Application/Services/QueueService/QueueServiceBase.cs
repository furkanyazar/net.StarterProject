using Newtonsoft.Json;

namespace Application.Services.QueueService;

public abstract class QueueServiceBase : IAsyncDisposable
{
    protected string SerializeMessage<T>(T message)
    {
        return message switch
        {
            string s => s,
            _ => JsonConvert.SerializeObject(message),
        };
    }

    protected T DeserializeMessage<T>(string messageJson)
    {
        if (typeof(T) == typeof(string))
            return (T)(object)messageJson;

        if (typeof(T) == typeof(int))
            return (T)(object)int.Parse(messageJson);

        return JsonConvert.DeserializeObject<T>(messageJson)!;
    }

    public abstract Task DeclareQueueAsync(
        string queueName,
        bool durable = true,
        bool exclusive = false,
        bool autoDelete = false
    );

    public abstract Task DeleteQueueAsync(
        string queueName,
        bool ifUnused = false,
        bool ifEmpty = false
    );

    public abstract Task PublishAsync<T>(
        string queueName,
        T message,
        long? delayMilliseconds = null
    );

    public abstract Task BindConsumerAsync<T>(
        string queueName,
        Func<T, Task> handleMessageAsync,
        bool autoAck = false,
        bool enableOrdering = false
    );

    public abstract ValueTask DisposeAsync();

    public abstract string QueueNamePrefix { get; set; }
}
