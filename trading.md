# Our Approach to Algorithmic Trading

This perhaps is most evident in decentralized finance as turing complete distributed ledgers produce an unprecedented combinatorial complexity of interveaved financial products. Deployed smart contracts all interact with each other directly within onchain transactions or indirectly through the market dynamics they promote.

Traditionally, traders looked at the market, identified patterns, and then formulated strategies to exploit those patterns for a profit.

Such produced strategies often require constant maintenance and monitoring, with some of them only being profitable. Further, dynamics of peer to peer networks create a lot of edge cases that simply are hard to reason about as a human.

Our approach is model based that is answering the _why_ of effects. Such models can start of as the sole assumption of an invariant repeatable pattern in market data.

Our approach to algorithmic trading is centered around identifying constants in a chaotic world. With constants we mean a causal structure that mediates action and effect. Auction systems, smart contracts, human nature, and physical reality can be unambiguously and automatically reasoned about using those models.

With usage of the [do-calculus](https://en.wikipedia.org/wiki/Causal_model), we can derive algorithms that automate traditionally manual work.

a) Identification of those causal models that fit past observations can be automated by minimizing surprise in future observations when compared to their expectations. Further, models are filtered by simplicity. This largely follows the free energy principle.

b) Probability distributions of future observations are fully derived from those causal models plus past observations, with all predictions carry the exact same assumptions as the model itself.

c) Causal models are used to reason backwards in time (_"I observed a trade, therefore there existed two matching limit orders"_) and forwards in time (_"When I place this market order now, I expect my balance to change"_). That allows for referencing unmeasurable or imaginary objects to set optimization goals, and nicely maps into reinforcement learning by picking the action that maximizes expected reward.

d) Large language models are utilized to annotate the causal dependencies with natural language so they are easier to understand for traders who verify and monitor them.

Using this combination of causal inference and model-based reinforcement learning allows for a generalized algorithm that crawls through possible actions, estimating their effects and evaluating them in terms of the trader wants. Causal-semantic models are fused with historic observations, together with the traders preferences for future observations serve as input to that +EV-Optimizer. It then crawls the action space defined by the used domain models, estimating their reward by causally reasoning through action and effect with the most +EV action path being executed at all points in time.

So instead of indentifying an imperative strategy with a positive expected value, a traders manual effort is shifted towards gaining a testable understanding of the world. This results in an ever growing library of domain models which describe the virtual and actual world in increasing detail and accuracy. It utilizes available compute to identify models from huge amounts of observational data, with humans guiding the computer on where to look first.

Additionally we think that this ontology can change trading as we know it. Away from blind profit seeking, towards coordination of surplus. This is because an ontology about action and effect and further the sensory-motor cycle allows to explicitly model the dynamics of organizations by being explcit about the underlying social coordination.

Based on that ontology, we created a market structure in which action and effect are traded directly, essentially negotiating who does what with the goal of coordinating towards surplus. And it's surplus that creates profit in the first place. Read more about coordination markets [here](https://github.com/BrunoZell/ask.network).

When being explicit about an organizations underlying social coordination, long term investments in potentially productive output can be reasoned about by counterfactual reasoning. Due to the nature of causal models, the theories used can still be pretty abstract, but they definitely are reproducible and make all assumptions explicit. This paves the way towards algorithmically allocated venture capital.

Further we expect that cross-organizational algorithmic trading coordinated through such coordination markets are more efficient than individual organizations trading in isolation from each other as action spaces are merged and enable strategies that couldn't be pulled off by either one of the participating organizations.
