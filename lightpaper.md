## Observations

Data is our window into reality. All data in Ask Network is modelled as _Messages_. Messages can occur in the virtual as network traffic between computers, and in the actual as causal effects between physical systems. As cultural capital, messages are captured and preserved by default.

Software can only capture virtual messages as digital data and does not have direct access to causal effects in reality. To capture real world data, hardware sensors must be utilized. We assume that all real world sensors are communicating their measurements as virtual messages via networking protocols. Thus, an Ask Network node records network protocol sessions only, with interpretations of reality being applied after consensus.

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

From here onwards, we call an active economic participant, those beings or things operating a sensory-motor cycle, an _agent_. Further, let's call all agents that are beings _subjects_, and all agents that are things _objects_.

We assume all agents aim to optimize their behavior to best respect the desires and aversions of their own. We refer to such an aim as the agents _wants_. While each agent certaily incorporates wants in their decision process as there always is a trade off between picking one action over another, the agent may not be explicitly aware of the wants that drive his decision.

All agents are assumed to do whatever they wants at any given moment by default. But beings and things may influence each other with their acts, positively or negatively. This soon will give rise to coordination between agents. For that, agents communicate an unambiguous shared world view, and negotiate who does what directly.

This section defines a general scheme for such a  conversation to semi-automatically negotiate social coordination directly in terms of conditional actions (_"I do this if I observe that"_), from which then commitment schemes are constructed (_"I do this if you do that"_).

The goals for this conversaition is:

- (i) We recognize that agents carry their own individual wants and each subject should be heard
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

Which translates into scanning the corpus of _proposals_ for action paths that increase the probability of _wants_ being satisfied according to future observations.

After desired action paths have been identified, users commit to them by signing its actions and publishing it as _commitments_. A commitment is an official statement of executing a specified act when the attached condition was satisfied by the latest observations.

Then the subjects go ahead and actually do it accordingly, or they won't, depending in their final decision in the moment of the act. Subjects may purposefuly record evidence to later prove they caused certain economic activity.

Wants are evaluated continuously on all observations as a measure for economic health. Fulfilled wants are indicators of realized value, possibly from successful coordination. Unfulfilled critical wants are indicators of unsuccessful coordination taking place.

## Economics

This section analyzes the dynamics of active participants interacting with each. First we'll analyze interactions in the actual, and then show how communication and the usage of the coordination conversation defined in the previous section can leave everyone better off.

Each agent can or can't do a given action. The group of all actions available to an agent is called its _action space_. What the agent ended up doing is called his _acts_.

Action spaces of agents can be increased by using other economic objects as an _instrument_, such as a car.

Acts yield effects in actuality. We quantify the expected effect of an action by comparing its causal expectations to those of inaction. All differing probabilities in the probed context compared to the inaction context must be caused by the probed action, under the same assumptions of the domain models used for reasoning.

Actions can be chained together into action paths, with previous actions changing reality so to make the next action possible. Individual actions in action paths may not be directly desirable, but the effect of a depending action could be desirale enough to make it worthwhile to pursue still.

We differentiate between two areas of interactions: _governance_ and _production_.

**Governance** is about what not to do. Or to rephrase, to make value destruction less likely. It is concerned with conflict prevention (ex-ante) and conflict correction (ex-post). It deals with practically mutually exclusive wants across individual agents and is used for final settlement after delivery.

**Production** is about what to do. Or to rephrase, to make value construction more likely. It is concerned with coordination towards mutually beneifitial action plans. After some rounds of agents updating commitments we should gradually improve the projected rate of filled wants as agents tend to go for opportunities of positive-sum cooperation.

## Coordination Markets

## Tokenomics

_Work in progress. Feel free to ask for more information._

Summary: A voucher economy is constructed which lets the user navigate within a generalization-specialization spectrum of productive services. On the one extreme, there is $ASK which represents a general purpose voucher for everything offerable. Users can specialize their amount of $ASK into more specialized vouchers, constraining the use of the tokens to just the service offerings that match the voucher definition. On the other extreme, we have a fully specialized voucher which defines a specific service offering from a specific service provider.
