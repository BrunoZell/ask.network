using Ask.Host.Persistence;
using Ask.Runtime.Modules.Execution;
using Ask.Runtime.Modules.Input;
using Ask.Runtime.Modules.Output;

namespace Ask.Runtime;
public class BrokerGroup
{
    private readonly StreamInput<NewDecision> _input;
    private readonly BrokerModule _executionModule;
    private readonly EmitOutput<ActionExecuted> _output;

    private BrokerGroup(
        StreamInput<NewDecision> input,
        BrokerModule executionModule,
        EmitOutput<ActionExecuted> output)
    {
        _input = input;
        _executionModule = executionModule;
        _output = output;
    }

    public static BrokerGroup Build(
        IReadOnlyDictionary<Type, object> broker,
        IHostPersistence persistence,
        IPlatformMessaging messaging)
    {
        var input = new StreamInput<NewDecision>(messaging);
        var executionModule = new BrokerModule(broker, persistence, input.Output);
        var output = new EmitOutput<ActionExecuted>(messaging, executionModule.Output);

        return new(input, executionModule, output);
    }

    public async Task Run(CancellationToken shutdown)
    {
        var inputTask = _input.Run(shutdown);
        var executionTask = _executionModule.Run(shutdown);
        var outputTask = _output.Run();

        await Task.WhenAll(inputTask, executionTask, outputTask);
    }
}
