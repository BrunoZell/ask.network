using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AskFi.Runtime.Persistence;
using AskFi.Runtime.Platform;
using static AskFi.Runtime.DataModel;
using static AskFi.Sdk;

namespace AskFi.Runtime.Modules.Execution;

internal class BrokerMultiplexer
{
    private readonly IReadOnlyDictionary<Type, object> _brokers;
    private readonly IPlatformPersistence _persistence;

    public BrokerMultiplexer(
        IReadOnlyDictionary<Type, object> brokers,
        IPlatformPersistence persistence)
    {
        _brokers = brokers;
        _persistence = persistence;
    }

    public bool TryStartActionExecution(DataModel.Action action, [NotNullWhen(true)] out Task<ActionExecutionResult>? actionExecution)
    {
        if (!_brokers.TryGetValue(action.ActionType, out var broker)) {
            // No broker available that can handle this type of action
            actionExecution = null;
            return false;
        }

        // Uses reflection over dynamic to support brokers that implement multiple IBroker<A> interfaces.
        // Perf: Precompile this delegate once at startup.
        var execute = typeof(BrokerMultiplexer).GetMethod(nameof(ExecuteAction), BindingFlags.Static | BindingFlags.NonPublic)!;
        var executeA = execute.MakeGenericMethod(action.ActionType);
        var invoke = executeA.Invoke(obj: null, new object[] { broker, action.ActionCid, _persistence });

        if (invoke is Task<ActionExecutionResult> i) {
            actionExecution = i;
            return true;
        }

        Debug.Fail("Return type changed unexpectedly");
        actionExecution = null;
        return false;
    }

    private static async Task<ActionExecutionResult> ExecuteAction<TAction>(
        IBroker<TAction> broker,
        ContentId actionCid,
        IPlatformPersistence persistence)
    {
        // Immediately yields back to ensure runtime does not block while action is executed.
        await Task.Yield();

        // Load action instructions into memory
        var action = await persistence.Get<TAction>(actionCid);

        var initiationTimestamp = DateTime.UtcNow;

        try {
            // Execute action using user-provided IBroker-instance.
            await broker.Execute(action);

            // Todo: pass user defined execution trace
            var trace = ActionExecutionTrace.NewSuccess(Microsoft.FSharp.Core.FSharpOption<byte[]>.None);
            var completionTimestamp = DateTime.UtcNow;
            return new ActionExecutionResult(trace, initiationTimestamp, completionTimestamp);
        } catch (Exception ex) {
            // Exception in user code
            var trace = ActionExecutionTrace.NewError(ex.ToString());
            var completionTimestamp = DateTime.UtcNow;
            return new ActionExecutionResult(trace, initiationTimestamp, completionTimestamp);
        }
    }
}
