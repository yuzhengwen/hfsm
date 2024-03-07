using System;

namespace YuzuValen.HFSM
{
    public static class StateMachineExtensions
    {
        public static void AddTransition<TStateId>(this StateMachine<TStateId> fsm, TStateId from, TStateId to, Func<bool> condition = null)
        {
            fsm.AddTransition(new Transition<TStateId>(from, to, condition));
        }
        public static void AddTransitionFromAny<TStateId>(this StateMachine<TStateId> fsm, TStateId to, Func<bool> condition = null)
        {
            fsm.AddTransition(new Transition<TStateId>(default, to, condition));
        }
        public static void AddTriggerTransition<TStateId>(this StateMachine<TStateId> fsm, string trigger, TStateId from, TStateId to)
        {
            fsm.AddTriggerTransition(trigger, new Transition<TStateId>(from, to));
        }
        public static void AddTriggerTransitionFromAny<TStateId>(this StateMachine<TStateId> fsm, string trigger, TStateId to)
        {
            fsm.AddTriggerTransition(trigger, new Transition<TStateId>(default, to));
        }

        /// <summary>
        /// Automatically adds a transition from 'to' to 'from' with the opposite condition
        /// </summary>
        /// <typeparam name="TStateId"></typeparam>
        /// <param name="fsm"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="condition"></param>
        public static void AddTwoWayTransition<TStateId>(this StateMachine<TStateId> fsm, TStateId from, TStateId to, Func<bool> condition = null)
        {
            fsm.AddTransition(new Transition<TStateId>(from, to, condition));
            fsm.AddTransition(new Transition<TStateId>(to, from, () => !condition()));
        }

        /// <summary>
        /// Extension method to add a new empty state to the state machine
        /// </summary>
        /// <typeparam name="TStateId"></typeparam>
        /// <param name="fsm"></param>
        /// <param name="id"></param>
        public static void AddState<TStateId>(this StateMachine<TStateId> fsm, TStateId id)
        {
            fsm.AddState(new BaseState<TStateId>(id));
        }
    }
}
