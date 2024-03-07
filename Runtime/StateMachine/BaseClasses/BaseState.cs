using System;

namespace YuzuValen.HFSM
{
    public class BaseState<TStateId>
    {
        public float exitTime = 0;
        /// <summary>
        /// Reference to the parent state machine <br/>
        /// NULL if current state is the top level state machine
        /// </summary>
        public StateMachine<TStateId> parent = null;
        public readonly TStateId id;
        public BaseState(TStateId id)
        {
            this.id = id;
        }
        /// <summary>
        /// Runs only once when state is added to the state machine<br/>
        /// Can be used to add transitions relevant to this state
        /// </summary>
        public virtual void Init() {}
        /// <summary>
        /// Run every time fsm enters this state
        /// </summary>
        public virtual void OnEnter() { }
        /// <summary>
        /// Run every time fsm exits this state
        /// </summary>
        public virtual void OnExit() { }
        /// <summary>
        /// Run on every fsm update
        /// </summary>
        public virtual void Update() { }
        /// <summary>
        /// Run on every fsm fixed update
        /// </summary>
        public virtual void FixedUpdate() { }

        public virtual void TriggerEvent(string eventName, EventArgs args=null) { }
    }
}