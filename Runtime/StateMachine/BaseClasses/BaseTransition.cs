
namespace YuzuValen.HFSM
{
    public abstract class BaseTransition<TStateId> 
    {
        public readonly TStateId to, from;
        public readonly bool forceInstantly;
        public BaseTransition(TStateId from, TStateId to, bool forceInstantly = false)
        {
            this.to = to;
            this.from = from;
            this.forceInstantly = forceInstantly;
        }
        public virtual bool ShouldTransition() => true;
        public virtual void BeforeTransition() { }
        public virtual void AfterTransition() { }
    }
}
