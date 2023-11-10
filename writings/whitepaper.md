# Ask Network Whitepaper [Draft]

## Domain Models

For a domain model to be considered, it must specify the following:

- The observation space$O_D$specific to the domain, including the types of observations that can be made.
- The action space$A_D$specific to the domain that defines what users can do within the chosen domain model.
- The rules that govern how the user's actions expectedly transform their state, captured in a structured causal model.

Such a domain model interface is already implemented as part of the [Ask Finance SDK](https://github.com/BrunoZell/ask.network/blob/askfi/AskFi.Sdk/AskFi.Sdk.fs). Any F# code implementing these interfaces and function signatures qualify as a domain model.

## User Accounts

Let's consider$N$users$U_i$indexed by$i$, each with:

- a domain model$D_i$, implying the users observation space$O_i$and action space$A_i$
- an environment state$s_i \in S$, containing all historic observations and actions sorted by time of occurrence.
- a policy$\pi_i$the user declared to follow

## Happenings

Each users virtual agent continuously observes the world with the human acting within it. Observations and actions, whether hypothetical or measured, are grouped togeher into a happening$H_i$and sorted into a state-vector:

$$H_i = O_i \cup A_i$$
$$s_i = [H_{i1}, H_{i2}, H_{i3}, ..., H_{in}]$$

where each$H_{ij}$is a happening for user$U_i$at time$j$.

## Policies

Each user commits to a policy$\pi_i$, which represents a decision process over the actions he is fine with.

### Policy Interface

This representation of a policy$\pi_i$for user$A_i$is used by other parts of the theory if the specific implementation of the strategy is not relevant. Whether a policy is implemented as deterministic or stochastic, it ultimately results in a mapping from a state$s \in S$to a single action$a \in A_i$, where$A_i$is users$i$action space as defined by the domain$D$he subscribes to.

$$\pi_i: S \to A_i$$

### Deterministic Policy

A deterministic policy can be defined as a mapping from the state space to the action space, where each state is associated with exactly one action:

$$\pi_i^D: S \rightarrow A_i$$

Under a deterministic policy, for each state$s \in S$, there is a unique action$a \in A_i$such that$\pi_i^D(s) = a$.

Deterministic policies are easier to compute, easier to reason about, and good enough for simple games. This is how a deterministic policy is implemented:

$$\pi^D_i(s) = \begin{cases}
a_{1} & \text{if } C_1(s) \text{ is true} \\
a_{2} & \text{if } C_2(s) \text{ is true} \\
\vdots \\
a_{n} & \text{if } C_n(s) \text{ is true} \\
\end{cases}$$

### Stochastic Policy

A stochastic policy produces a probability distribution over the set of possible actions rather than a single action. That probability distribution is then sampled to determine a single action$a_i$to take:

$$\pi_i^S: S \rightarrow \Delta(A_i)$$

where$\Delta(A_i)$denotes the set of all possible probability distributions over the action space$A_i$. For each state$s \in S$,$\pi_i^S(s)$is a probability distribution over the actions.

To implement a stochastic policy$\pi^S_i$that maps states to probability distributions over actions, we modify the deterministic policy implementation to return probability distributions over the set of possible actions.

$$\pi^S_i(s) = \begin{cases}
P(a_1 | s) & \text{if } C_1(s) \text{ is true} \\
P(a_2 | s) & \text{if } C_2(s) \text{ is true} \\
\vdots \\
P(a_n | s) & \text{if } C_n(s) \text{ is true} \\
\end{cases}$$

In this case,$P(a_j | s)$represents the probability distribution over actions given the state$s$, when the condition$C_j(s)$is true. Since$\pi^S_i(s)$defines a probability distribution, the sum of probabilities for all actions given a state should equal 1:

$$\sum_{a \in A_i} P(a | s) = 1$$

To give a more concrete example, let's define a simple stochastic policy for a game where the action space consists of two actions: moving left or right. The state could be the position of the player on a track, and the conditions might be based on the proximity to the finish line:

$$\pi^S_i(s) = \begin{cases}
\{ \text{left: } 0.3, \text{ right: } 0.7 \} & \text{if } s \text{ is close to the finish on the left} \\
\{ \text{left: } 0.8, \text{ right: } 0.2 \} & \text{if } s \text{ is close to the finish on the right} \\
\{ \text{left: } 0.5, \text{ right: } 0.5 \} & \text{if } s \text{ is far from the finish} \\
\end{cases}$$

This example shows that for each state$s$, depending on where the player is relative to the finish line, the policy$\pi^S_i(s)$will return a probability distribution over the possible actions "left" and "right". The policy is stochastic because it does not prescribe a single action but rather a distribution from which an action is sampled.

## Trajectories

When a policy$\pi_i$is evaluated against a certain environment$s_i$, it results in a samplable probability distribution over all the actions user$i$declared to do or not to do, which is unrolled into a trajectory$\tau$.

A trajectory$\tau$is unrolled by repeatedly applying the policy of user$U_i$to that users state$s_i$until a steady-state is reached. For the initial$s_i$the users latest observed state-vector is used, which produces a new virtual state$s^prime_i$.

$$\tau_i = \left[ (s_{i0}, a_{i0}), (s_{i1}, a_{i1}), (s_{i2}, a_{i2}), \ldots, (s_{iT}) \right]$$

A trajectory may incorporate other users actions if they are relevant to fulfil the the asks of the trajectory owner$U_i$.

## Evaluations of Asks

Each user has a set of binary asks that can either be fulfilled (1) or unfulfilled (0). These asks can be combined using logical operations to form composite asks.

### Binary Asks

Let$a_i$be the set of binary asks declared by user$i$, where each ask$a_{ij}$is a function with signature:

$$a_{ij} : S \rightarrow \{0, 1\}$$

which evaluates to 1 if the$j$-th ask is fulfilled in state$s$and 0 otherwise. The implementation is expressed as:

$$a_{ij}(s) = q(s)$$

Where:

-$s$is the current state of the user.
-$q$is any user-defined binary query on a state. The user takes care of this transformation being a semantically accurate representation of his values.
-$a_{ij}(s)$returns 1 if the ask is fulfilled and 0 otherwise.

### Weighted Preference Score

Preference ordering is a way to express how different outcomes (or states) are valued relative to each other based on the user's subjective criteria.

A preference ordering$W$can be represented as a function that assigns a numerical value to each possible state based on the user's preferences, with a higher numerical value indicating a higher-valued state:

$$W_i(s) : S \rightarrow \mathbb{R}$$

where:

-$S$is the set of all possible states representing the environment.
-$W_i(s)$is the preference score of state$s$according to user$i$, which is a real number.

Each user assigns weights to their asks$a_i = (a_{i1}, a_{i2}, ..., a_{in_i})$to indicate their relative importance, creating a preference ordering$W_i$for user$i$over all possible states$s$:

$$W_i(s) = \sum_{j=1}^{n_i} w_{ij} \cdot a_{ij}(s)$$

where:

-$a_{ij}(s)$indicates the state of the$j$-th ask for user$i$in state$s$,
-$w_{ij}$is the weight assigned to the$j$-th ask by user$i$,
-$n_i$is the number of asks for user$i$.

### Composite Asks

Composite asks are constructed using logical operations on binary asks. For example, a composite ask using exclusive-or to represent "I want either A or B, but not both" looks like:

$$a_{ij}(s) = a_{ik}(s) \oplus a_{il}(s)$$

where$\oplus$represents the XOR operation.

This function will have a value of 1 if either$a_{ik}(s)$or$a_{il}(s)$is true, but not both, and a value of 0 otherwise. The users preference score$P(s)$could be modified to include this new feature:

$$P(s) = w_1 \cdot a_1 + w_2 \cdot a_2 + w_{XOR} \cdot a_{XOR}$$

where$w_{XOR}$is the weight representing the importance of the XOR condition being satisfied.

### Cumulative Utility over a Trajectory

We'll define a cumulative utility function$Q_i$for user$i$that captures the fulfillment of asks over a trajectory$\tau$:

$$Q_i(\tau_i) = \sum_{t=0}^{T} W_i(s_{it})$$

where:

-$T$is the time horizon over which the trajectory is considered, with an episode ending when a steady economic state is reached or when the user$i$died, making him inable to act.
-$W_i(s_t)$is the preference score of user$i$at time$t$in state$s_t$.

While this does aggregate all different kinds of asks into a single dimension again (specific to a users set of asks), the structure of the asks defined through semantic-causal queries on the users state still gives more detail about what it is that the user perceived as cost or value. So even if monetarily your final balance would be the same, the user still might have incurred other types of costs identified by certain asks being unfulfilled in the trajectory. Asks are an additional discriminator for picking one trajectory over another even if weighted utility (for a single state) or cumulative utility (for a whole trajectory) is indifferent. This allows for full cost accounting, and attribution of first-principle values.

## Bargaining Solution Space

### Disagreement Point

Let$d = \tau$represent the disagreement point, which is the outcome if no user changes his policy$\pi$. The currently declared policy is what each user falls back to if no further agreements are made:

$$d_i \in \pi_i$$

#### Conditional Actions:
Users commit to actions based on conditions that relate to their perspective (state) of the environment. These are conditional policies:

$$\pi_i^c(s) = \{ a_i | a_i \text{ is executed if condition } c(s) \text{ is true} \}$$

#### Agreement and Execution:
The trajectory of state-action pairs$\tau$agreed upon by the users can be defined as a sequence of state-action pairs$(s_0, a_0), (s_1, a_1), ..., (s_T, a_T)$, where each$a_t$is derived from the conditional policies$\pi_i^c(s)$.

#### Execution Verification:
Each action's effects are measured and verified against the commitment:

$$V(s_{t+1}, a_t) \to \{ \text{True, False} \}$$
Where$V$is a verification function that checks if the action$a_t$at state$s_t$led to the expected state$s_{t+1}$.

This mathematical structure allows for the formal analysis and simulation of the bargaining process within a structured, causal environment. Users learn and adapt their policies through both reinforcement learning and causal inference, continually refining their strategies to achieve the best possible outcomes according to their individual utility functions.
