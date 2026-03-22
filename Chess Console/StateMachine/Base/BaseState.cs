
namespace Chess_Console.StateMachine
{
    internal abstract class BaseState : IDisposable
    {
        protected ISwitchableState? _switchable = default;

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
    }
}
