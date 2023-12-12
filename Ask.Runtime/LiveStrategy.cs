using Ask.Host.Persistence;
using Ask.Runtime.Modules.Input;
using Ask.Runtime.Modules.Output;
using Ask.Runtime.Modules.Perspective;
using Ask.Runtime.Modules.Strategy;
using static Ask.Sdk;

namespace Ask.Runtime;

public class LiveStrategy
{
    private readonly StreamInput<NewKnowledgeBase> _input;
    private readonly ObservationDeduplicationModule _observationDeduplication;
    private readonly StrategyModule _strategyModule;
    private readonly EmitOutput<NewDecision> _output;

    private LiveStrategy(
        StreamInput<NewKnowledgeBase> input,
        ObservationDeduplicationModule observationDeduplication,
        StrategyModule strategyModule,
        EmitOutput<NewDecision> output)
    {
        _input = input;
        _observationDeduplication = observationDeduplication;
        _strategyModule = strategyModule;
        _output = output;
    }

    public static LiveStrategy Build(
        Func<Reflection, Context, Decision> strategy,
        IHostPersistence persistence,
        IPlatformMessaging messaging)
    {
        var input = new StreamInput<NewKnowledgeBase>(messaging);
        var observationDeduplicator = new ObservationDeduplicationModule(persistence, input.Output);
        var strategyModule = new StrategyModule(strategy, persistence, observationDeduplicator.Output);
        var output = new EmitOutput<NewDecision>(messaging, strategyModule.Output);

        return new(input, observationDeduplicator, strategyModule, output);
    }

    public async Task Run(CancellationToken shutdown)
    {
        var inputTask = _input.Run(shutdown);
        var observationTask = _observationDeduplication.Run(shutdown);
        var strategyTask = _strategyModule.Run(shutdown);
        var outputTask = _output.Run();

        await Task.WhenAll(inputTask, observationTask, strategyTask, outputTask);
    }
}
