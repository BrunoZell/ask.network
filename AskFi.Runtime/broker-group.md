# Runtime Component: Broker Group

Listen to `NewDecision` messages are executed accordingly, given the required `IBroker`-instances are set up for this module instance.

## Processing Pipeline

1. Input `NewDecision`
2. Executor Module
3. Output `ActionExecuted`
