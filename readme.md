# Ask Network - better than money

Ask Network defines a conversational structure as an alternative solution to social coordination. We expect this to be more efficient, effective, and humane than money or every other political system humanity tried out yet.

A technical introduction can be found in the [lightpaper](lightpaper.md).

## Value Proposition

The network helps to coordinate who does what so that people get what they want at a lower cost, or at a better quality.

It achieves a high quality of service by standardizing the expression of wants and aversions (as Asks), essentially outsourcing market analysis and offer selection to professional market makers with just the final purchasing decision being signed by the user.

However, the efficiency gain only shines through once the network has a large user base. Until then, the code can be used to run trading bots. If you want to do that, get familiar with the [AskFi SDK](https://github.com/BrunoZell/AskFi.Sdk) and [AskFi Runtime](https://github.com/BrunoZell/AskFi.Runtime).

## Simple description of the solution

We're building an exchange on which you trade social coordination directly, in terms of what you do and don't do. From that, a new paradigm of coordination markets emerges.

It's based on the idea of a match, essentially complementary statements of _"I do this if you do that"_ with all participants accepting the plan.

The network provides an auction mechanism to negotiate matches, accounting of open interest from accepted matches, verification of physical delivery, and according settlement in the virtual.

## Definition of the problem

Basing all economic activity on pairwise transactions is not always the most efficient or effective conversation to have about who does what and when.

Also, the matching process between service providers and benefitiaries in current economies usually is solved externally, with typical approaches like advertisements creating a lot of mental friction for consumers.

Further, using written natural languages to define work contracts and service offerings makes them hard to automatically analyze and adapt.

And modern smart contract platforms force you to define imperative procedures within a virtual machine, making pragmatic representations of the physical world, where most value and capital resides, unnecessary hard or impossible. They also create an oracle problem by forcing all external data to be imported through a limiting transaction format and often come with complex rules for transaction sequencing.

## Solution

Instead of pairwise transactions, Ask Network accounts for future economic activity as conditional actions (_"I do this if I observe that"_), and past economic activity is based on observations of the outer world. From that, onchain reasoning quantifies expected (future) and actual (past) changes to reality caused by the accounted economic activity. Further, these changes can be evaluated against users Asks (wants or aversions) to quantify whether the economic activity has helped.

Matchmaking and counterparty-discovery is done by market makers who search for and propose possibly valuable matches, which are then automatically filtered, prioritized, and annotated based on how the users Asks (wants or aversions) are expectedly satisfied by it before affected users make the final call of committing to them or not.

Asks and service offerings are defined in terms of causal-semantic models of reality which enable onchain reasoning about effects of different possible action paths, and whether these effects are desired or not.

To ensure all desired economic activity can be accounted for regardless how everyday life changes over the next centuries, the network is invariant to how users view the world and what values they pursue in that world. That also makes it interoperable with existing coordination mechanisms like companies, money, laws, political institutions, and text messages.

It utilizes a data model that can express every possible representation of the world, with the state transition function being reality itself.

- [Structured Causal Models](https://www.google.de/books/edition/Causality/wnGU_TsW3BQC) are used to unambiguously communicate expected effects of actions in reality.
- [CQL (Categorical Query Language)](https://journals.plos.org/plosone/article?id=10.1371/journal.pone.0024274) is used to migrate observed sensory information between different perspectives on the same thing, essentially communicating between differing world views.
- Annotations of data types in [IEML (Information Economy MetaLanguage)](https://intlekt.io/2022/10/02/semantic-computing-with-ieml-3/) makes for interoperable and computable semantics, making much of the matchmaking procedure automatable.
- Using [HGTP (the Hypergraph Transfer Protocol)](https://docs.constellationnetwork.io/learn) for validation ensures compatibility with existing L1s and physical reality since HGTPs data model is compatible with _any data type_. Thus it's possible to refer to capital that exists in the outer world and to validate physical delivery.

All of these technologies have foundations in abstract mathematics, which makes them inherently compatible and if puzzled together coherently, pose a general solution to the general problem of social coordination, as simple as possible.

Read our lightpaper [here](lightpaper.md).
