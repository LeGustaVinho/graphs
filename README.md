# Graphs

Graphs can be used to create any data structure that is based on graphs (including Trees).

Features:

- Defines data structure for the graph itself, nodes and connections
- Connections can be unidirectional or bidirectional
- The graph can be hierarchical, allowing to place a graph inside another graph
- Strongly typed, extensible and already has several common methods for working with graphs, nodes and connections.
- Allows graphs in trees

Dependencies:

- [ Legendary Tools - Commons](https://github.com/LeGustaVinho/legendary-tools-common " Legendary Tools - Commons")

### How to use

A short example of how to create a State Machine using Graphs:

#### Creating State using LinkedNode class

```csharp
public class State<TState,TTrigger> : LinkedNode<StateMachine<TState, TTrigger>, 
        State<TState,TTrigger>, 
        StateConnection<TState,TTrigger>, 
        StateConnectionContext<TTrigger>>
    {
        public readonly TState Name;

        public State(TState name, StateMachine<TState, TTrigger> owner = null) : base(owner)
        { }
	}
```
Note that the LinkedNode in its generic types need to know the type of the State Machine itself, the State and the State Connection, this happens to guarantee the type conversion of the .

#### Creating State Connection using NodeConnection class

```csharp
public class StateConnection<TState, TTrigger> : 
        NodeConnection<StateMachine<TState, TTrigger>, 
        State<TState, TTrigger>, 
        StateConnection<TState, TTrigger>, 
        StateConnectionContext<TTrigger>>
    {
        public StateConnection(TTrigger trigger,
            State<TState, TTrigger> from,
            State<TState, TTrigger> to,
            NodeConnectionDirection direction = NodeConnectionDirection.Bidirectional,
            float weight = 0) : base(from, to, direction, weight)
        {
            Context.Trigger = trigger;
        }
	}
```
#### Creating StateMachine using LinkedGraph class

```csharp
 public class StateMachine<TState, TTrigger> : LinkedGraph<StateMachine<TState, TTrigger>, 
        State<TState, TTrigger>, 
        StateConnection<TState, TTrigger>, 
        StateConnectionContext<TTrigger>>
    {

        public StateMachine(string name, TState anyStateName, State<TState, TTrigger> state = null) : base(state)
        {
            Name = name;
            AnyState = new State<TState, TTrigger>(anyStateName, this, true);
        }
	}
```
The complete implementation of the State Machine using Graphs can be consulted in this repository, [Legendary Tools - State Machine](https://github.com/LeGustaVinho/state-machine "Legendary Tools - State Machine").

