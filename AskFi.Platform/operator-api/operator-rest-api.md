# Operator REST API

Todo: Turn into OpenAPI spec

Operations:

```text
GET /op/observer/list
POST /op/observer           { observers: [#observer-bybit; #observer-polygon] }
GET /op/observer/{observer-id}
DELETE /op/observer/{observer-id}

..Brokers
..Sequencers
```

Domain:

```text
GET /domain/observer/list
POST /domain/observer     { assembly: bytes; type: string; } -> #Observer
```
