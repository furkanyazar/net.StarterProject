using System.Text;
using Application.Services.QueueService;
using Core.CrossCuttingConcerns.Logging.Abstraction;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.Adapters.QueueService;

public class RabbitMQQueueServiceAdapter : QueueServiceBase
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly ILogger _logger;

    private const int MaxRetryCount = 3;
    private const string RetryHeaderKey = "x-retry-count";
    private readonly string _delayedExchangeName = "delayed-exchange";
    private readonly string _deadLetterExchangeName = "dead-letter-exchange";
    private readonly string _deadLetterQueueName = "dead-letter-queue";

    public override string QueueNamePrefix { get; set; }

    private RabbitMQQueueServiceAdapter(
        IConnection connection,
        IChannel channel,
        ILogger logger,
        string prefix
    )
    {
        QueueNamePrefix = prefix;
        _connection = connection;
        _channel = channel;
        _logger = logger;
    }

    public static async Task<RabbitMQQueueServiceAdapter> CreateAsync(
        IConfiguration configuration,
        ILogger logger,
        CancellationToken cancellationToken = default
    )
    {
        const string rabbitMQConfigurationSection = "RabbitMQConfiguration";
        RabbitMQConfiguration rabbitMQConfiguration =
            configuration.GetSection(rabbitMQConfigurationSection).Get<RabbitMQConfiguration>()
            ?? throw new InvalidOperationException(
                $"\"{rabbitMQConfigurationSection}\" section cannot found in configuration."
            );

        string prefix = rabbitMQConfiguration.Prefix;

        var factory = new ConnectionFactory
        {
            HostName = rabbitMQConfiguration.HostName,
            UserName = rabbitMQConfiguration.UserName,
            Password = rabbitMQConfiguration.Password,
        };

        IConnection connection = await factory.CreateConnectionAsync(cancellationToken);
        IChannel channel = await connection.CreateChannelAsync(
            cancellationToken: cancellationToken
        );

        var adapter = new RabbitMQQueueServiceAdapter(connection, channel, logger, prefix);
        await adapter.InitializeTopologyAsync(cancellationToken);
        return adapter;
    }

    private async Task InitializeTopologyAsync(CancellationToken cancellationToken = default)
    {
        await _channel.ExchangeDeclareAsync(
            exchange: _delayedExchangeName,
            type: "x-delayed-message",
            durable: true,
            autoDelete: false,
            arguments: new Dictionary<string, object?> { { "x-delayed-type", "direct" } },
            cancellationToken: cancellationToken
        );

        await _channel.ExchangeDeclareAsync(
            exchange: _deadLetterExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            cancellationToken: cancellationToken
        );

        await _channel.QueueDeclareAsync(
            queue: _deadLetterQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                { "x-message-ttl", 1000 * 60 * 60 * 24 * 7 },
            },
            cancellationToken: cancellationToken
        );

        await _channel.QueueBindAsync(
            _deadLetterQueueName,
            _deadLetterExchangeName,
            _deadLetterQueueName,
            cancellationToken: cancellationToken
        );
    }

    public override async Task DeclareQueueAsync(
        string queueName,
        bool durable = true,
        bool exclusive = false,
        bool autoDelete = false
    )
    {
        var queueArgs = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", _deadLetterExchangeName },
            { "x-dead-letter-routing-key", _deadLetterQueueName },
        };

        if (!queueName.StartsWith(QueueNamePrefix))
        {
            queueName = $"{QueueNamePrefix}{queueName}";
        }

        await _channel.QueueDeclareAsync(
            queue: queueName,
            durable: durable,
            exclusive: exclusive,
            autoDelete: autoDelete,
            arguments: queueArgs
        );

        await _channel.QueueBindAsync(
            queue: queueName,
            exchange: _delayedExchangeName,
            routingKey: queueName
        );

        _logger.Information($"Queue declared and bound to exchange: {queueName}");
    }

    public override async Task PublishAsync<T>(
        string queueName,
        T message,
        long? delayMilliseconds = null
    )
    {
        var properties = new BasicProperties
        {
            Persistent = true,
            Headers = new Dictionary<string, object?>(),
        };

        if (delayMilliseconds.HasValue)
        {
            properties.Headers["x-delay"] = delayMilliseconds.Value;
        }

        int retryCount = 0;
        if (!properties.Headers.ContainsKey(RetryHeaderKey))
        {
            properties.Headers[RetryHeaderKey] = retryCount;
        }

        string serializedMessage = SerializeMessage(message);
        var body = Encoding.UTF8.GetBytes(serializedMessage);

        if (!queueName.StartsWith(QueueNamePrefix))
        {
            queueName = $"{QueueNamePrefix}{queueName}";
        }

        await _channel.BasicPublishAsync(
            exchange: _delayedExchangeName,
            routingKey: queueName,
            mandatory: false,
            basicProperties: properties,
            body: body
        );

        _logger.Information(
            $"Message published to queue {queueName}. RetryCount={retryCount}, Delay={delayMilliseconds}"
        );
    }

    public override async Task BindConsumerAsync<T>(
        string queueName,
        Func<T, Task> handleMessageAsync,
        bool autoAck = false,
        bool enableOrdering = false
    )
    {
        if (!queueName.StartsWith(QueueNamePrefix))
        {
            queueName = $"{QueueNamePrefix}{queueName}";
        }

        if (enableOrdering)
        {
            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);
            _logger.Information(
                $"Ordering enabled for queue: {queueName}. PrefetchCount set to 1."
            );
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(body);
            var headers = ea.BasicProperties.Headers;

            _logger.Information(
                $"Message received from queue {queueName}. DeliveryTag={ea.DeliveryTag}"
            );

            try
            {
                T deserialized = DeserializeMessage<T>(messageJson);
                await handleMessageAsync(deserialized);

                if (!autoAck)
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);

                _logger.Information(
                    $"Message processed successfully. DeliveryTag={ea.DeliveryTag}"
                );
            }
            catch (Exception e)
            {
                if (!autoAck)
                {
                    int currentRetry = 0;

                    if (headers != null && headers.TryGetValue(RetryHeaderKey, out var raw))
                    {
                        if (raw is byte[] bytes)
                            currentRetry = int.TryParse(
                                Encoding.UTF8.GetString(bytes),
                                out var parsed
                            )
                                ? parsed
                                : 0;
                        else if (raw is int i)
                            currentRetry = i;
                    }

                    if (currentRetry < MaxRetryCount)
                    {
                        var retryProps = new BasicProperties
                        {
                            Persistent = true,
                            Headers = new Dictionary<string, object?>
                            {
                                { RetryHeaderKey, currentRetry + 1 },
                                { "x-delay", 1000 * 60 * 5 * (currentRetry + 1) },
                            },
                        };

                        var retryBody = Encoding.UTF8.GetBytes(messageJson);

                        await _channel.BasicPublishAsync(
                            exchange: _delayedExchangeName,
                            routingKey: queueName,
                            mandatory: false,
                            basicProperties: retryProps,
                            body: retryBody
                        );

                        _logger.Warning(
                            $"Message processing failed. Will retry. RetryCount={currentRetry + 1}, Queue={queueName}, DeliveryTag={ea.DeliveryTag}, Exception={e.Message}"
                        );
                    }
                    else
                    {
                        var dlqProps = new BasicProperties
                        {
                            Persistent = true,
                            Headers = new Dictionary<string, object?>
                            {
                                { RetryHeaderKey, currentRetry },
                            },
                        };

                        var deadBody = Encoding.UTF8.GetBytes(messageJson);

                        await _channel.BasicPublishAsync(
                            exchange: _deadLetterExchangeName,
                            routingKey: _deadLetterQueueName,
                            mandatory: false,
                            basicProperties: dlqProps,
                            body: deadBody
                        );

                        _logger.Error(
                            $"Message failed after maximum retries and sent to DLQ. Queue={queueName}, DeliveryTag={ea.DeliveryTag}, Exception={e.Message}"
                        );
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
            }
        };

        await _channel.BasicConsumeAsync(queue: queueName, autoAck: autoAck, consumer: consumer);
    }

    public override async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }

        _logger.Information("RabbitMQ channel and connection closed.");

        GC.SuppressFinalize(this);
    }

    public override async Task DeleteQueueAsync(
        string queueName,
        bool ifUnused = false,
        bool ifEmpty = false
    )
    {
        if (!queueName.StartsWith(QueueNamePrefix))
        {
            queueName = $"{QueueNamePrefix}{queueName}";
        }

        await _channel.QueueDeleteAsync(queue: queueName, ifUnused: ifUnused, ifEmpty: ifEmpty);
    }
}
