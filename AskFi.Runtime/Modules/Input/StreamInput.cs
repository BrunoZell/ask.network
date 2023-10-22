using System.Threading.Channels;
using AskFi.Runtime.Platform;

namespace AskFi.Runtime.Modules.Input;
internal class StreamInput<TMessage>
{
    private readonly IPlatformMessaging _messaging;
    private readonly Channel<TMessage> _input;

    public ChannelReader<TMessage> Output => _input.Reader;

    public StreamInput(IPlatformMessaging messaging)
    {
        _messaging = messaging;
        _input = Channel.CreateUnbounded<TMessage>();
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        await foreach (var input in _messaging.Listen<TMessage>(cancellationToken)) {
            await _input.Writer.WriteAsync(input);
        }
    }
}
