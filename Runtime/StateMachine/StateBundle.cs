using System.Collections.Generic;

namespace YuzuValen.HFSM
{
    /// <summary>
    /// Contains the State object and all Transitions (and TriggerTransitions) from this state
    /// </summary>
    /// <typeparam name="TStateId"></typeparam>
    public class StateBundle<TStateId>
    {
        public BaseState<TStateId> state;
        public readonly List<BaseTransition<TStateId>> transitionsFrom = new();
        public readonly Dictionary<string, List<BaseTransition<TStateId>>> triggerTransitionsFrom = new();

        public StateBundle(BaseState<TStateId> state)
        {
            this.state = state;
        }
    }
}