namespace Chess_Console.StateMachine
{
    internal abstract class BaseState : IDisposable
    {
        protected readonly ISwitchableState _switchable;

        #region IDisposable

        public void Dispose()
            => UnSubscribeFromEvents();

        #endregion

        public BaseState(ISwitchableState switchable)
        {
            _switchable = switchable;
        }

        public abstract void Start();

        public abstract void Stop();

        protected virtual void SubscribeToEvents() { }

        protected virtual void UnSubscribeFromEvents() { }

        public override string ToString()
            => "\n--- Went to state: " + GetType().Name + " ---";
    }
}
