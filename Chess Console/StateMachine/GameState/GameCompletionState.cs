using Chess_Console.Data.Enums;
using Chess_Console.Others;

namespace Chess_Console.StateMachine.GameState
{
    internal class GameCompletionState : BaseState
    {
        #region Events

        protected override void SubscribeToEvents()
        {
            GameEvents.Instance.OnGameCompletion += RevealWinner;
        }

        protected override void UnSubscribeFromEvents()
        {
            GameEvents.Instance.OnGameCompletion -= RevealWinner;
        }

        #endregion

        public GameCompletionState(ISwitchableState switchable) : base(switchable)
        {

        }

        public override void Start()
        {
            SubscribeToEvents();
        }

        public override void Stop()
        {
            UnSubscribeFromEvents();
        }

        private void RevealWinner(ChessSide winnerSide)
        {
            Console.WriteLine($"Match is over! {winnerSide} has won!");
        }
    }
}
