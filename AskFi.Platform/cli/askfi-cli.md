# askfi CLI

```text
askfi query <file> <type> <function> [--live]
askfi backtest start <file> <type> <function> <from> <to> [--watch]
askfi baktest view <backtest-id> [--watch]

askfi simulation start <file> <type> <function> [--watch]
askfi simulation view <simulation-id> [--watch]

askfi execute <file> <type> <function>
askfi execute view <execution-id> [--live]

askfi login
askfi account // shows credit line, realized pnl
```

---

### `askfi query --help`

```plaintext
Usage: askfi query <file> <type> <function> [--live]

Description:
  Sends the query code from the specified file to the platform for compilation and verification. If the --live option is provided, the results will be streamed in real-time.

Arguments:
  <file>      Path to the F# code file containing the query.
  <type>      Name of the F# type in the uploaded code file.
  <function>  Function name to be executed.
  --live      (Optional) Stream the query results in real-time.

Example:
  askfi query myQuery.fs MyQueryType getPrices --live
```

---

### `askfi backtest start --help`

```plaintext
Usage: askfi backtest start <file> <type> <function> <from> <to> [--watch]

Description:
  Initiates a backtest on the platform using the strategy code from the specified file over the given time range.

Arguments:
  <file>      Path to the F# code file containing the strategy.
  <type>      Name of the F# type in the uploaded code file.
  <function>  Function name to be executed.
  <from>      Start date for the backtest in YYYY-MM-DD format.
  <to>        End date for the backtest in YYYY-MM-DD format.
  --watch     (Optional) Monitor the backtest in real-time.

Example:
  askfi backtest start myStrategy.fs MyStrategyType executeStrategy 2022-01-01 2022-12-31 --watch
```

---

### `askfi backtest view --help`

```plaintext
Usage: askfi backtest view <backtest-id> [--watch]

Description:
  View the results of a specific backtest.

Arguments:
  <backtest-id>   ID of the backtest to view.
  --watch         (Optional) Monitor the backtest in real-time.

Example:
  askfi backtest view 12345 --watch
```

---

### `askfi simulation start --help`

```plaintext
Usage: askfi simulation start <file> <type> <function> [--watch]

Description:
  Starts a simulation on the platform using the strategy code from the specified file.

Arguments:
  <file>      Path to the F# code file containing the strategy.
  <type>      Name of the F# type in the uploaded code file.
  <function>  Function name to be executed.
  --watch     (Optional) Monitor the simulation in real-time.

Example:
  askfi simulation start myStrategy.fs MyStrategyType simulateStrategy --watch
```

---

### `askfi simulation view --help`

```plaintext
Usage: askfi simulation view <simulation-id> [--watch]

Description:
  View the results of a specific simulation.

Arguments:
  <simulation-id>   ID of the simulation to view.
  --watch           (Optional) Monitor the simulation in real-time.

Example:
  askfi simulation view 67890 --watch
```

---

### `askfi execute --help`

```plaintext
Usage: askfi execute <file> <type> <function>

Description:
  Deploys a live strategy on the platform.

Arguments:
  <file>      Path to the F# code file containing the strategy.
  <type>      Name of the F# type in the uploaded code file.
  <function>  Function name to be executed.

Example:
  askfi execute myStrategy.fs MyStrategyType executeStrategy
```

---

### `askfi execute view --help`

```plaintext
Usage: askfi execute view <execution-id> [--live]

Description:
  View the results of a specific live strategy execution.

Arguments:
  <execution-id>   ID of the live strategy execution to view.
  --live           (Optional) Monitor the execution in real-time.

Example:
  askfi execute view 78901 --live
```

---

### `askfi login --help`

```plaintext
Usage: askfi login

Description:
  Authenticates the user with the platform.

Example:
  askfi login
```

---

### `askfi account --help`

```plaintext
Usage: askfi account

Description:
  Retrieves the user's account details from the platform, including credit line and realized PnL.

Example:
  askfi account
```
