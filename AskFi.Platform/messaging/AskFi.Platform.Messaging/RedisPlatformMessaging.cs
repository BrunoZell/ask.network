using System.Runtime.CompilerServices;
using System.Threading.Channels;
using AskFi.Runtime.Platform;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AskFi.Runtime.Messaging;

public class RedisPlatformMessaging : IPlatformMessaging, IAsyncDisposable
{
    private readonly ConnectionMultiplexer _redis;
    private readonly ILogger? _logger;

    public RedisPlatformMessaging(
        string endpoint,
        ILogger? logger = null)
    {
        _redis = ConnectionMultiplexer.Connect(
            new ConfigurationOptions {
                EndPoints = { endpoint },
            });
        _logger = logger;
    }

    public RedisPlatformMessaging(
        EndPointCollection endpoints,
        ILogger? logger = null)
    {
        _redis = ConnectionMultiplexer.Connect(
            new ConfigurationOptions {
                EndPoints = endpoints,
            });
        _logger = logger;
    }

    public void Emit<TMessage>(TMessage message)
    {
        var channel = GetRedisChannelName<TMessage>(); // throws if TMessage is not a platform message
        var publisher = _redis.GetSubscriber();
        var textMessage = JsonConvert.SerializeObject(message);
        publisher.Publish(channel, textMessage, CommandFlags.FireAndForget);
    }

    public async IAsyncEnumerable<TMessage> Listen<TMessage>([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var channel = GetRedisChannelName<TMessage>(); // throws if TMessage is not a platform message
        var subscriber = _redis.GetSubscriber();
        var queue = Channel.CreateUnbounded<TMessage>();
        var subscription = Task.Run(() => subscriber.SubscribeAsync(channel, async (channel, message) => {
            if (message.IsNullOrEmpty) {
                return;
            }

            try {
                var newMessage = JsonConvert.DeserializeObject<TMessage>(message!);

                if (newMessage is null) {
                    return;
                }

                await queue.Writer.WriteAsync(newMessage);
            } catch (Exception ex) {
                _logger?.LogError(
                    ex,
                    "An error occurred during the deserialization of a received message. Target type was {TMessage} with raw message payload of {message}",
                    typeof(TMessage).FullName,
                    message);
            }
        }));

        try {
            await foreach (var message in queue.Reader.ReadAllAsync(cancellationToken)) {
                yield return message;
            }
        } finally {
            await subscriber.UnsubscribeAllAsync();
            await subscription;
        }
    }

    private static string GetRedisChannelName<TMessage>()
    {
        if (typeof(TMessage).Namespace != "AskFi.Runtime.Messages") {
            throw new InvalidOperationException("Only AskFi messages can be subscribed to.");
        }

        var channel = typeof(TMessage).Name;
        return channel;
    }

    public async ValueTask DisposeAsync()
    {
        await _redis.DisposeAsync();
    }
}
