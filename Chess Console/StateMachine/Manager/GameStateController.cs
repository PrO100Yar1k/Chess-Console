using Chess_Console.Views;
using Chess_Console.StateMachine.Base;
using Chess_Console.StateMachine.GameState;

namespace Chess_Console.StateMachine.Manager
{
    internal class GameStateController : ISwitchableState, IDisposable
    {
        private List<BaseState> _allStates;
        private BaseState _currentState;

        #region IDisposable

        public void Dispose()
        {
            foreach (IDisposable state in _allStates)
                state.Dispose();
        }

        #endregion

        public GameStateController()
        {
            InitializeStateControllers();
        }

        private void InitializeStateControllers()
        {
            _allStates = new List<BaseState>() {
                new MainMenuState(this),
                new GameplayState(this),
                new GameCompletionState(this)
            };
        }

        public bool CheckStateForActivity<State>() where State : BaseState
        {
            return _currentState == _allStates.FirstOrDefault(s => s is State);
        }

        public void SwitchState<State>() where State : BaseState
        {
            BaseState state = _allStates.FirstOrDefault(s => s is State);
            DisplayView.WriteLine($"{state}");

            _currentState?.Stop();
            _currentState = state;
            _currentState.Start();
        }
    }
}
