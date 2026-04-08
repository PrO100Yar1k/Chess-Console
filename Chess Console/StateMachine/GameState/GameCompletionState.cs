using Chess_Console.Core.Common.Structures;
using Chess_Console.Infrastructure.Common;
using Chess_Console.StateMachine.Base;
using Chess_Console.Core.Common.Enums;
using Chess_Console.Views;

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

        private void RevealWinner(GameCompletionResult result)
        {
            string statusMessage = result.Type switch
            {
                GameCompletionType.Win or GameCompletionType.Defeat => $"{result.WinnerSide} has dominated the field!",
                GameCompletionType.Draw => "DRAW! No one could claim victory.",
                GameCompletionType.Stalemate => "STALEMATE! The game is locked.",
                _ => "Game Over."
            };

            DisplayView.WriteLine($"\n{statusMessage}");

            DisplayView.WriteLine($"Reason: {result.Message}");

            if (result.WinnerSide.HasValue)
                Console.WriteLine($"\nMatch is over! {result.WinnerSide} is the winner!");
        }
    }
}
