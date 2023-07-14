# Ask Network Lightpaper Draft

## Abstract

This document poses a conversational structure as an alternative solution to social coordination. We expect this to be more efficient, effective, and humane than money or every other political system humanity tried out yet.

The network helps to coordinate who does what so that people get what they want at a lower cost, or at a better quality.

Coordination markets achieve a high quality of service by standardizing the expression of wants and aversions, essentially outsourcing market analysis and offer selection to professional market makers with just the final purchasing decision being signed by the user.

The rest of this document walks through the design of the network with the most fundamental part first, toward more complex structures.

## Observations

Data is our window into reality. All data in the network is modelled as _messages_. Messages can occur in the virtual as network traffic between computers, and in the actual as causal effects between physical systems. As cultural capital, messages are captured and preserved by default.

Software can only capture virtual messages as digital data and does not have direct access to causal effects in reality. To capture real world data, hardware sensors must be utilized. We assume that all real world sensors are communicating their measurements as virtual messages via networking protocols. Thus, every node records network protocol sessions only, with interpretations of reality being applied after consensus.

## Domains

That message history is used to derive reproducible representations of the actual and virtual external world. Such reproducible representations are specified as domains that take observations as input, and derive conclusions about the past, present, and future external world, subject to explicit assumptions about causal relationships.

At each domains core lies a causal graph. One can instantiate a causal graph by integrating observations into explicitly assumed causal relationships, producing a context. On that context, it can be reasoned backwards in time (I observed this, therefore that must have happened), and forwards in itme (if I do this, expectedly that will happen).

Once conclusions about hidden variables or counterfactuals where made, they can be queried through a semantic query interface. Nodes in the causal graphs carry semantically computable annotations in IEML. Queries are standard CQL expressions on the uniform data model of a semantic-causal context, combined with the domains semantic annotations in the formulation of the query.

Domain modellers combine observation procedures, interpretation, authentication, causal assumptions, and semantic annotations into interoperable domain modules, producing an ever growing library of domains that model reality increasingly accurate.

Specifying representations of reality in standardized and thus interoperable causal-semantic models enables transferrable reasoning about the behavior of virtual and actual external systems (hidden variables), and unobserved but possible observations or actions (counterfactuals).

Furthermore, the network provides a continuous integration pipeline for domain modellers by testing their causal theories against real world data.

Essentially it is an accounting system for everything measurable, with an attached unambiguous reasoning machine for everything unmeasurable.

## Network of Beings and Things

Expressing semantic annotations in IEML ensures that all conceptualization has exact positions in the semantic sphere, which allows for rich analysis across domains.

Most importantly for social coordination is the identification of beings and things, with all of them being embedded in the same physical reality, although at different positions in the geospatial process. What follows is a virtual representation of the actual economy that is acted out, through which past economic activity can be analyzed and future economic activity arranged.

Let us first differentiate economic participants from everything else. An economic participant is everything and everybody that influences voluntary participants to a degree that the influenced participant accounts for. Essentially it's the set of everything that is directly or indirectly important to at least one voluntary participant, with voluntary participant referring to a being that uses Ask Network to coordinate.

Let us further differentiate between passive and active participants:

Passive participants are things with no behavior of their own other than the natural laws that govern their physical body. Desired things are economic goods (for example, a glass of wine) whereas undesired things are economic costs (like waste).

Active participants are those beings or things that operate a sensory-motor cycle and posess decision making abilities over their action space. Senses (like eyes and ears) or sensors (like cameras or microphones) on their body are employed to capture sensory information, which further is interpreted and reasoned about with their mental capacity, and finally acted out in their motor system (like legs or electric motors). Notably, such a situational awareness implies each active participant has certain desires and aversions, regardless of them being self-conscious or not. Picking one action over another always comes with a tradeoff about what has happened and what could have happened.

Now let's distinguish between communicating and silent participants. Communicating participants are those that have mastered languages to a degree that allows them to communicate with other speakers of the language in use, whereas silent participants can't. Communicating participants are first of all humans, and second of all smart things with embedded computers that communicate through networking protocols.

We identify a human as every communicating participant which is not labeled as a thing. Simply because we haven't yet observed any animal being able to make use of internet protocols.

Humans can authenticate themselves by claiming spots on the economic social graph. Smart things can authenticate themselves with an onboard private key.

## Coordination

From here onwards, we call an active economic participant an _agent_, those beings or things operating a sensory-motor cycle.

We assume all agents aim to optimize their behavior to best respect the desires and aversions of their own. We refer to such an aim as the agents _wants_. While each agent certaily incorporates wants in their decision process as there always is a trade off between picking one action over another, the agent may not be explicitly aware of the wants that drive his decision.

All agents are assumed to do whatever they want at any given moment by default. But agents may influence each other with their acts, positively or negatively. This soon will give rise to voluntary coordination between agents. For that, agents communicate an unambiguous shared world view, and negotiate who does what directly.

This section defines a general scheme for such a conversation to semi-automatically negotiate social coordination directly in terms of conditional actions (_"I do this if I observe that"_), from which then commitment schemes are constructed (_"I do this if you do that"_).

The goals for this conversaition is:

- (i) We recognize that agents carry their own individual wants and each agent should have a voice
- (ii) We also would welcome an open and unambiguous conversation about what we can do, esentially searching through our possible action plans.
- (iii) Further, an unambiguous social contract with automatic accounting and trustless settlement could help to coordinate on what we do.

The conversation defines three communication channels:

- (i) Broadcasting of wants by each agent
- (ii) Broadcasting of potential action plans
- (iii) Declaring of own commitment to conditional actions

With three types of messages, respectively:

- (i) **Wants**, answering: **how do we like it?**
  
  _(Represented as a signed binary query (fulfilled or not) in a given context.)_

- (ii) **Proposals**, answering: **what could we do?**

  _(Represented as changes (additions and removals) to active commitments of one or more agents.)_
  
- (iii) **Commitments**, answering: **what will we do?**

  _(A signed conditioal action. Conditions are either satisfied or not in a given context. The action choice is derived from the matching context via a specified query.)_

Recall that the public observation pool together with applied domain theories answers: **how is it?**

Those queries can be combined into a formalization of the foundational question of social coordination:

**what could we do | so we like it | in the future?**

Which translates into scanning the corpus of _proposals_ for actions that increase the probability of _wants_ being satisfied according to future observations.

After desired actions have been identified, users commit to them by signing and publishing according _commitments_. A commitment is an official statement of executing a specified act when the attached condition was satisfied by the latest observations.

Then the agents go ahead and actually do it accordingly, or they won't, depending in their final decision in the moment of the act. Agents may purposefuly record evidence to later prove they caused or haven't caused certain economic activity.

Wants are evaluated continuously on all observations as a measure for economic health. Fulfilled wants are indicators of realized value, possibly from successful coordination. Unfulfilled critical wants are indicators of unsuccessful coordination taking place.

## Economics

This section analyzes the dynamics of active participants interacting with each. First we'll analyze interactions in the actual, and then show how coordination can leave practically everyone better off.

Each agent can or can't do a given action. The group of all actions available to an agent is called its _action space_. What the agent ended up doing is called his _acts_.

Action spaces of agents can be increased by using other economic objects as an _instrument_, such as a car.

Acts yield effects in actuality. We quantify the expected effect of an action by comparing its causal expectations to those of inaction. All differing probabilities in the probed context compared to the inaction context must be caused by the probed action, under the same assumptions of the domain models used for reasoning.

Actions can be chained together into action plans, with previous actions changing reality so to make the next action possible. Individual actions in action plans may not be directly desirable, but the effect of a depending action could be desirale enough to make it worthwhile to pursue still.

We differentiate between two areas of interactions: _governance_ and _production_.

**Governance** is about what not to do. Or to rephrase, to make value destruction less likely. It is concerned with conflict prevention ex-ante by negotiating compromises and conflict correction ex-post use of counter-acts. It deals with practically mutually exclusive wants across individual agents and is used for final settlement after delivery.

**Production** is about what to do. Or to rephrase, to make value construction more likely. It is concerned with coordination towards mutually beneifitial action plans. After some rounds of agents updating their commitments, the collective action plan should gradually improve the projected rate of filled wants as agents tend to go for opportunities of positive-sum cooperation.

## Coordination Markets

Let's generalize the order book: Define _asks_, _offers_, and _matches_.

**Asks** represent the demand side of productive output and are published by consumers. They specify what services they currently look out for, formulated in the consumers perspective abstracting out all to him irrelevant details of production and delivery.

**Offers** represent the supply side of productive output and are published by service providers according to their organizations production capacity.

Proposed **Matches** claim one or more ask would get filled if a specified list of offers would get ordered.

Asks and offers are authored in different perspectives, that of the consumer or service provider, respectively. The market makers task is to translate between those perspectives, finding out when to order what service offering to expectedly fulfill asks of one or more users.

Now let's generalize a futures product with physical delivery. For any offer to be listed in the networks offer book, it must implement an abstract coordination interface: offer -> order -> deal -> production -> delivery -> settlement -> settled.

Each offer defines an order formular. When a user likes to order a service, he will fill out the offers order formular, sign it and send it to the service provider. The service provider analyzes the order and if we likes to provide that service, he signs a deal acknowledging and accepting that order. A newly signed deal initially is in an active state and comparable to open interest in a traditional futures contract.

An offers underlying commitment scheme consists of commitments conditioned on signed deals of that very offer. Therefore, once the deal is signed and published, worker agents orchestrated by the service provider automatically commit to actions that make up the service.

First, there is a production phase in which worker agents prepare everything for a successful delivery. When production is done, the benefitiary will be notified that the product is ready for delivery. The condition for this notification is defined in the original offer and can only be changed if the active deal is renegotiated.

Next, the delivery phase is interactive involving both workers and benefitiaries. Each offer outlines usage instructions to benefitiaries describing what to do when in order to have an easy experience and a successful delivery.

When delivery is done, as defined in the terms of the deal, the settlement phase begins. This is where payments are released or disputes are raised, all in line with the terms defined in the active deal.

After a delay proportional to the economic size of the deal relative to other deals in the network, the deal is moved into its final state of being closed.

## Tokenomics

_Work in progress. Feel free to ask for more information._

Summary: A voucher economy is constructed which lets the user navigate within a generalization-specialization spectrum of productive services. On the one extreme, there is $ASK which represents a general purpose voucher for everything offerable. Users can specialize their amount of $ASK into more narrow vouchers, constraining the use of the tokens to just the service offerings that match the voucher definition. On the other extreme, we have a fully specialized voucher which is tied to a specific service offering from a specific service provider.

## Theoretical Foundations

To ensure all desired economic activity can be accounted for regardless how everyday life changes over the next centuries, the network is invariant to how users view the world and what wants they pursue in that world. That also makes it interoperable with existing coordination mechanisms like companies, money, laws, political institutions, and text messages.

It utilizes a data representation that can express every possible representation of the world, with the state transition function being reality itself.

- [Structured Causal Models](https://www.google.de/books/edition/Causality/wnGU_TsW3BQC) are used to unambiguously communicate expected effects of actions in reality.
- [CQL (Categorical Query Language)](https://journals.plos.org/plosone/article?id=10.1371/journal.pone.0024274) is used to migrate observed sensory information between different perspectives on the same thing, essentially communicating between differing world views.
- Labelling semantic-causal nodes in [IEML (Information Economy MetaLanguage)](https://intlekt.io/2022/10/02/semantic-computing-with-ieml-3/) makes for interoperable and computable semantics, making much of the matchmaking procedure automatable.
- Using [HGTP (the Hypergraph Transfer Protocol)](https://docs.constellationnetwork.io/learn) for validation ensures compatibility with existing L1s and physical reality since HGTPs data model is compatible with any data type. Thus it's possible to refer to capital that exists in the outer world and to validate physical delivery.

All of these technologies have foundations in abstract mathematics, which makes them inherently compatible and if puzzled together coherently, pose a general solution to the general problem of social coordination, as simple as possible.

## Next steps

Implementations of the observation pool and local semantic-causal reasoning are in the works. First it is used for algorithmic trading. Once the software is stable, the public network is implemented which lets trading bots across organizational boundaries coordinate with each other, creating a global market making botnet and possibly a p2p crypto exchange. Further, domain modules that model physical reality are authored to gradually enable negotiation of social coordination in terms of measurable and unmeasurable reality without the need for traditional financial products or accounting procedures.

There are two ways you can contribute:

You can **help implementing this** using your technical expertise and dedication in exchange for a minimum viable life (MVL). [Text me](https://t.me/extremecookie) if you are interested.

Alternatively, you can **donate crypto assets** to pay for the MVLs of our contributors. Please send crypto assets to our SAFE on Ethereum: [`0x5cf266bb7b5EFE578C2C63A14Ea2b644710E153e`](https://app.safe.global/balances?safe=eth:0x5cf266bb7b5EFE578C2C63A14Ea2b644710E153e). If you want access to the multisig, just ask.
