using Ask.Runtime.Messages;
using Ask.Runtime.Modules.Input;
using Ask.Runtime.Modules.Output;
using Ask.Runtime.Modules.Perspective;
using Ask.Runtime.Platform;

namespace Ask.Runtime;

public class KnowledgeBaseGossip
{
    private readonly StreamInput<NewKnowledgeBase> _input;
    private readonly ObservationDeduplicationModule _perspectiveMergeModule;
    private readonly EmitOutput<NewKnowledgeBase> _output;

    private KnowledgeBaseGossip(
        StreamInput<NewKnowledgeBase> input,
        ObservationDeduplicationModule perspectiveMergeModule,
        EmitOutput<NewKnowledgeBase> output)
    {
        _input = input;
        _perspectiveMergeModule = perspectiveMergeModule;
        _output = output;
    }

    public static KnowledgeBaseGossip Build(
        IPlatformPersistence persistence,
        IPlatformMessaging messaging)
    {
        var input = new StreamInput<NewKnowledgeBase>(messaging);
        var observationDeduplication = new ObservationDeduplicationModule(persistence, input.Output);
        var output = new EmitOutput<NewKnowledgeBase>(messaging, observationDeduplication.Output);

        return new(input, observationDeduplication, output);
    }

    public async Task Run(CancellationToken shutdown)
    {
        var inputTask = _input.Run(shutdown);
        var perspectiveTask = _perspectiveMergeModule.Run(shutdown);
        var outputTask = _output.Run();

        await Task.WhenAll(inputTask, perspectiveTask, outputTask);
    }
}
