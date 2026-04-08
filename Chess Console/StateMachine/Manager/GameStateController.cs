using Chess_Console.StateMachine.Base;
using Chess_Console.StateMachine.GameState;

using Microsoft.Extensions.DependencyInjection;

namespace Chess_Console.StateMachine.Manager
{
    internal class GameStateController : ISwitchableState, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly List<BaseState> _allStates;
        private BaseState _currentState;

        #region IDisposable

        public void Dispose()
        {
            foreach (IDisposable state in _allStates)
                state.Dispose();
        }

        #endregion

        public GameStateController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _allStates = new List<BaseState>();
        }

        public void InitializeService()
        {
            _allStates.Add(_serviceProvider.GetRequiredService<MainMenuState>());
            _allStates.Add(_serviceProvider.GetRequiredService<GameplayState>());
            _allStates.Add(_serviceProvider.GetRequiredService<GameCompletionState>());
        }

        public bool CheckStateForActivity<State>() where State : BaseState
        {
            return _currentState == _allStates.FirstOrDefault(s => s is State);
        }

        public void SwitchState<TState>() where TState : BaseState
        {
            var state = _serviceProvider.GetRequiredService<TState>();

            _currentState?.Stop();
            _currentState = state;
            _currentState.Start();
        }
    }
}
