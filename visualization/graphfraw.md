What follows is a description of a type of diagram I want you to draw as a mermaid graph. You don't get the specific code to visualize yet, and you should not output any mermaid code yet.

Instead, read this diagram specification and come up with a good intermediate representation that specifies all data that must be visualized to conform with the specification. Put it as F# code thereby fully typing that intermediate representation.

This are the specs:


Please visualize each defined type with a box, containing the type name on the first row, and a max 7 word summary of what the type represents. On the first line, but the types name in bold text. On the second line within the box, put that short description in italics with a smaller font size.

For each visualized types, draw one outgoing arrow for each property of that type.
If the property type is a direct reference to another type, make the arrow go to that type and label it with the property name.
If the property type is a ContentId, check the attached comment specifying the type obtained by resolving that CID link and put a dashed arrow for that property to the type the ContentId resolves to, with the property name as the arrows label.
he F# code that follows.

There are few well-known primitives. Draw a box for each primitive and represent properties of other types that resolve to one of the primitives as an arrow to that primitive type box just as a direct type reference. Those well known primitives are:

- DateTime, System.DateTime: A point in time, an instant.
- Task, System.Task, ValueTask, System.ValueTask: An ongoing operation
- IAsyncEnumerable: A stream of new datums
- string
- option

Classify all boxes into:

- primitive type
- data model type
- sdk type
- domain type

Give the 'ActionSpace type a property specific_action_of resolving to 'Action.
Give the 'ObservationSpace type a property specific_observation_of resolving to 'Observation.

For each property (arrow) in the graph, classify whether it is pure data access or it represents an executable algorithm that runs on the type, and possibly resolves links.
Please differentiate between data access and executable algorithm not by text in the arrows labels, but instead by using different background-colors for that arrows label. Pick light colors.

Use this color palette for styling, and specify the class of every box.

classDef type fill:#F5F5F5,stroke:#A9A9A9;
classDef datamodel fill:#D5E8D4,stroke:#82B366;
classDef primitive fill:#DAE8FC,stroke:#6C8EBF;
classDef action fill:#FFF2CC,stroke:#D6B656 (for 'ActionSpace and 'Action)
classDef observation fill:#E1D5E7 (for 'ObservationSpace and 'Observation)
classDef domain fill:#E1D5E7 (for all domain types other than the action and observation-related ones)

> response with F# diagram data model

Okay cool. Now please use those definitions to build the data tree required to represent the following piece of F# code:

[code]

> response with diagram data model instantiation that represents the provided code

cool. Now draw it as a d3.js visualization where the boxes and arrows are rendered via SVG elements.

> [visualization]

I updated the code. Let's go through the new code and draw the according diagram from that.
