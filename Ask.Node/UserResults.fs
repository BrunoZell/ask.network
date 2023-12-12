namespace Ask.Node

open Ask.Host.Persistence

// References results from user-requested compute from RPC requests
type UserResults = {
    // Maps QuerySequenceHead.Identity as the query sequence id
    /// to the latest known QuerySequenceHead.Result
    /// Map<QuerySequenceHead.Identity, QuerySequenceHead.Result>
    QuerySequences: Map<ContentId<unit(*QuerySequenceHead*)>, ContentId<unit(*QuerySequenceHead*) > list>

    // Maps DecisionSequenceHead.Identity as the query sequence id
    /// to the latest known DecisionSequenceHead.Result
    /// Map<DecisionSequenceHead.Identity, DecisionSequenceHead.Result>
    DecisionSequences: Map<ContentId<unit(*DecisionSequenceHead*)>, ContentId<unit(*DecisionSequenceHead*) > list>

    // Execution sequences - live real acting
    // Just like DM.DecisionSequence but references Broker ExecutionId and indirectly the Brokers 'Response

    // Simulation sequences - life fake acting
    // A decision sequence weaved into an environment sequence

    // Backtest sequences - historic fake acting
    // A decision sequence weaved into an environment sequence
}
