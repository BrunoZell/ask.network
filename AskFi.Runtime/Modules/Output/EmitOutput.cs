using System.Threading.Channels;
using AskFi.Runtime.Platform;

namespace AskFi.Runtime.Modules.Output;
internal class EmitOutput<TMessage>
{
    private readonly IPlatformMessaging _messaging;
    private readonly ChannelReader<TMessage> _output;

    public EmitOutput(IPlatformMessaging messaging, ChannelReader<TMessage> output)
    {
        _messaging = messaging;
        _output = output;
    }

    public async Task Run()
    {
        await foreach (var output in _output.ReadAllAsync()) {
            _messaging.Emit(output);
        }
    }
}
