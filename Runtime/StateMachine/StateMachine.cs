using System;
using System.Collections.Generic;
using UnityEngine;

namespace YuzuValen.HFSM
{
    public class StateMachine<TStateId> : BaseState<TStateId>
    {
        #region current & initial state info
        protected TStateId initialStateId;
        protected TStateId currentStateId;
        protected BaseState<TStateId> currentState;
        protected StateBundle<TStateId> currentBundle;
        #endregion
        // List of all state bundle objects, accessible by ID
        public readonly Dictionary<TStateId, StateBundle<TStateId>> stateBundles = new();

        public Func<TStateId> decideState;

        private readonly List<BaseTransition<TStateId>> transitionsFromAny = new();
        private readonly Dictionary<string, List<BaseTransition<TStateId>>> triggerTransitionsFromAny = new();

        public StateMachine(TStateId id) : base(id)
        {
        }

        /// <summary>
        /// Set initial state of the state machine<br/>
        /// State machine will reset to this state every OnEnter call
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="Exception"></exception>
        public void SetInitialState(TStateId id)
        {
            if (!stateBundles.ContainsKey(id)) throw new Exception("Add state first!");
            initialStateId = id;
        }
        /// <summary>
        /// Provide a function that will decide the initial state during every OnEnter call
        /// </summary>
        /// <param name="decideState"></param>
        public void SetInitialStateOnEnter(Func<TStateId> decideState)
        {
            this.decideState = decideState;
        }

        #region inherited methods
        /// <summary>
        /// State machine will be active after calling this method
        /// </summary>
        public override void OnEnter()
        {
            if (decideState != null)
                SetInitialState(decideState());
            if (initialStateId == null)
                throw new Exception("Set initial state first!");
            currentStateId = initialStateId;
            currentBundle = stateBundles[initialStateId];
            currentState = currentBundle.state;
            currentState.OnEnter();
        }

        public override void OnExit()
        {
            currentState.OnExit();
        }

        public override void Update()
        {
            currentState.Update();
            CheckTransitions();
        }

        public override void FixedUpdate()
        {
            currentState.FixedUpdate();
        }
        #endregion
        public void OnEnter(TStateId id)
        {
            if (stateBundles.TryGetValue(id, out var bundle))
            {
                currentStateId = id;
                currentBundle = bundle;
                currentState = currentBundle.state;
                currentState.OnEnter();
            }
        }

        #region Public methods to Add Transition
        /// <summary>
        /// Adds a transition to the state machine<br />
        /// If from state = null, this adds a TransitionFromAny 
        /// </summary>
        /// <param name="transition"></param>
        public void AddTransition(BaseTransition<TStateId> transition)
        {
            if (EqualityComparer<TStateId>.Default.Equals(transition.from))
            {
                transitionsFromAny.Add(transition);
                return;
            }
            var bundle = stateBundles[transition.from];
            bundle.transitionsFrom.Add(transition);
        }

        /// <summary>
        /// Add transitions that are only checked on event trigger<br />
        /// If from state = null, this adds a TriggerTransitionFromAny
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="transition"></param>
        public void AddTriggerTransition(string trigger, BaseTransition<TStateId> transition)
        {
            if (EqualityComparer<TStateId>.Default.Equals(transition.from))
                if (triggerTransitionsFromAny.TryGetValue(trigger, out var list))
                    list.Add(transition);
                else
                    triggerTransitionsFromAny.Add(trigger, new List<BaseTransition<TStateId>> { transition });

            var bundle = stateBundles[transition.from];
            if (bundle.triggerTransitionsFrom.TryGetValue(trigger, out var list2))
                list2.Add(transition);
            else
                bundle.triggerTransitionsFrom.Add(trigger, new List<BaseTransition<TStateId>> { transition });
        }
        #endregion

        public void AddState(BaseState<TStateId> state)
        {
            stateBundles.Add(state.id, new StateBundle<TStateId>(state));
            state.parent = this;
            state.Init();
        }
        /// <summary>
        /// Will check for transitions linked to the trigger event
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="args">Used to pass any arguments to </param>
        public override void TriggerEvent(string trigger, EventArgs args = null)
        {
            // check for trigger transitions from any state
            if (triggerTransitionsFromAny.TryGetValue(trigger, out var list))
            {
                foreach (var transition in list)
                {
                    if (transition.ShouldTransition())
                    {
                        DoTransition(transition);
                        return;
                    }
                }
            }
            // check for trigger transitions from current state
            var currentBundle = stateBundles[currentStateId];
            if (currentBundle.triggerTransitionsFrom.TryGetValue(trigger, out var list2))
            {
                foreach (var transition in list2)
                {
                    if (transition.ShouldTransition())
                    {
                        DoTransition(transition);
                        return;
                    }
                }
            }
            // trigger event on current state 
            currentState.TriggerEvent(trigger, args);
        }

        private void CheckTransitions()
        {
            foreach (var transition in transitionsFromAny)
            {
                if (transition.ShouldTransition())
                {
                    DoTransition(transition);
                    return;
                }
            }
            foreach (var transition in currentBundle.transitionsFrom)
            {
                if (transition.ShouldTransition())
                {
                    DoTransition(transition);
                    return;
                }
            }
        }
        private void DoTransition(BaseTransition<TStateId> transition)
        {
            transition.BeforeTransition();
            RequestStateChange(transition.to);
            transition.AfterTransition();
        }
        /// <summary>
        /// Possible to directly request transition to a state <br/>
        /// </summary>
        /// <param name="stateId"></param>
        public void RequestStateChange(TStateId stateId)
        {
            if (currentState == null || stateId.Equals(currentStateId))
                return;
            if (currentState.exitTime < 0.001)
                SetState(stateId);
        }
        /// <summary>
        /// Recommended to use RequestStateChange instead <br/>
        /// </summary>
        /// <param name="stateId"></param>
        public void SetState(TStateId stateId)
        {
            if (stateBundles.TryGetValue(stateId, out var bundle))
            {
                currentState?.OnExit();
                currentStateId = stateId;
                currentBundle = bundle;
                currentState = currentBundle.state;
                currentState.OnEnter();
            }
            else
            {
                // check for state in parent sm until top level sm
                StateMachine<TStateId> temp = parent;
                while (temp != null)
                {
                    if (temp.stateBundles.ContainsKey(stateId))
                    {
                        // set parent sm to new state
                        // this will automatically exit all child states of the previous state
                        temp.SetState(stateId);
                        return;
                    }
                    temp = parent.parent;
                }
                // find a child state machine that contains the state
                BaseState<TStateId> child = RecursiveSearchChildrenInternal(this, stateId) ?? throw new Exception("State not found!");

                // set all parent of child to state machine that contains child
                TStateId childId = stateId;
                temp = child.parent;
                while (temp != this)
                {
                    temp.SetInitialState(childId);
                    child = temp;
                    childId = child.id;
                    temp = temp.parent;
                }
                temp.SetState(childId);
            }
        }
        /// <summary>
        /// gets a state from a child state machine (recursive)
        /// </summary>
        /// <param name="root">root of recursive search</param>
        /// <param name="stateId">id of state to search for</param>
        /// <returns></returns>
        private BaseState<TStateId> RecursiveSearchChildrenInternal(StateMachine<TStateId> root, TStateId stateId)
        {
            // check for state in current sm
            if (root.stateBundles.TryGetValue(stateId, out StateBundle<TStateId> res))
                return res.state;
            // else loop through all statemachines in state and repeat
            foreach (var bundle in root.stateBundles)
            {
                if (bundle.Value.state is StateMachine<TStateId> child)
                {
                    Debug.Log("Searching children of " + bundle.Key);
                    var res1 = RecursiveSearchChildrenInternal(child, stateId);
                    if (res1 != null)
                        return res1;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a list of current and all nested state IDs
        /// </summary>
        /// <returns></returns>
        public List<TStateId> GetAllCurrentStates()
        {
            List<TStateId> states = new();

            StateMachine<TStateId> temp = this;
            while (temp is StateMachine<TStateId>)
            {
                states.Add(temp.currentStateId);
                temp = temp.currentState as StateMachine<TStateId>;
            }
            return states;
        }
    }
}