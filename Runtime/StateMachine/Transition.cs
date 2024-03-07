using System;
using UnityEngine;

namespace YuzuValen.HFSM
{
    public class Transition<TStateId> : BaseTransition<TStateId>
    {
        private readonly Func<bool> condition;
        private readonly Action beforeTransition, afterTransition;

        /// <summary>
        /// Creates a new transition from one state to another
        /// </summary>
        /// <param name="to">State to transition from</param>
        /// <param name="from">State to transition to</param>
        /// <param name="condition">Condition to check. null by default -> will automatically transition</param>
        /// <param name="beforeTransition">Optional: Callback function called before transition</param>
        /// <param name="afterTransition">Optional: Callback function called after transition</param>
        /// <param name="forceInstantly">Ignore State exit time (default false)</param>
        public Transition(
            TStateId from,
            TStateId to,
            Func<bool> condition = null,
            Action beforeTransition = null,
            Action afterTransition = null,
            bool forceInstantly = false) : base(from, to, forceInstantly)
        {
            this.condition = condition;
            this.beforeTransition = beforeTransition;
            this.afterTransition = afterTransition;
        }
        /// <summary>
        /// If condition is null, transition will always be true<br />
        /// Otherwise, condition will be checked
        /// </summary>
        /// <returns></returns>
        public override bool ShouldTransition()
        {
            return condition == null || condition();
        }
        public override void BeforeTransition() => beforeTransition?.Invoke();
        public override void AfterTransition() => afterTransition?.Invoke();

    }
}
