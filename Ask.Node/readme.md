# Ask Node

A single-machine client hosting the Ask Runtime.

It implements `Ask.Host.Persistence.IHostPersistence` which the Ask Runtime depends on and which is used by the node itself to store app data.

It keeps a database on the local disk. It contains the `KnowledgeBase`, holding all observations, actions and sequence information.

Further it holds the `OperatorState`, which is a collection of managed resources managed by the node operator, which instruct the node what code to run.
