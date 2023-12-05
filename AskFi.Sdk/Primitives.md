# Primitives

## [IPLD](https://ipld.io/docs/data-model/kinds/)

```text
Null
Boolean
Integer
Float
String
Bytes
Link
List
Map
```

## [CUE](https://cuelang.org/docs/concepts/logic/#cues-hierarchy)

```text
⊤ (top)
bytes
string
bool
int
number
null
struct
list
⊥ (bottom)
```

## AskFi Primitives

| Primitive   | F# Type                                 | C# Type                          | IPLD Type      |
|-------------|-----------------------------------------|----------------------------------|----------------|
| boolean     | bool                                    | bool                             | Bool           |
| integer     | int                                     | int                              | Int            |
| decimal     | decimal                                 | decimal                          | **String**     |
| float       | float                                   | float                            | Float          |
| string      | string                                  | string                           | String         |
| bytes       | byte[]                                  | byte[]                           | Bytes          |
| link 'Datum | AskFi.ContentId<'Datum>                 | AskFi.ContentId<'Datum>          | Link           |
| option 'T   | Microsoft.FSharp.Core.Option<'T>        | System.Nullable<'T> or T?        | optional       |
