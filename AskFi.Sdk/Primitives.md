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

[Predefined bounds](https://cuelang.org/docs/tutorials/tour/types/bounddef/)

```text
uint      >=0
uint8     >=0 & <=255
int8      >=-128 & <=127
uint16    >=0 & <=65536
int16     >=-32_768 & <=32_767
rune      >=0 & <=0x10FFFF
uint32    >=0 & <=4_294_967_296
int32     >=-2_147_483_648 & <=2_147_483_647
uint64    >=0 & <=18_446_744_073_709_551_615
int64     >=-9_223_372_036_854_775_808 & <=9_223_372_036_854_775_807
int128    >=-170_141_183_460_469_231_731_687_303_715_884_105_728 &
              <=170_141_183_460_469_231_731_687_303_715_884_105_727
uint128   >=0 & <=340_282_366_920_938_463_463_374_607_431_768_211_455
```

## AskFi Primitives

| Primitive   | F# Type                                 | C# Type                          | IPLD Type      | CUE Type       | Bounds |
|-------------|-----------------------------------------|----------------------------------|----------------|----------------|--------|
| boolean     | bool                                    | bool                             | Bool           | bool           |        |
| integer     | int                                     | int                              | Int            | int            | ✅ |
| decimal     | decimal                                 | decimal                          | **String**     | **number** ⚠️ | ✅ |
| float       | float                                   | float                            | Float          | number         | ✅ |
| string      | string                                  | string                           | String         | string         |     |
| bytes       | byte[]                                  | byte[]                           | Bytes          | bytes          | ✅ |
| link 'Datum | AskFi.ContentId<'Datum>                 | AskFi.ContentId<'Datum>          | Link           | **bytes**      |     |
| option 'T   | Microsoft.FSharp.Core.Option<'T>        | System.Nullable<'T> or T?        | optional       | *_             |     |

Available bounds are> `>x <x >=x <=x`
