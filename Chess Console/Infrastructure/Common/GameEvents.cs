using Chess_Console.Core.Common.Structures;

namespace Chess_Console.Infrastructure.Common
{
    internal sealed class GameEvents
    {
        #region Lazy Initialization

        private static readonly Lazy<GameEvents> _lazy =
                new Lazy<GameEvents>(() => new GameEvents());

        public static GameEvents Instance => _lazy.Value;

        private GameEvents()
        {

        }

        #endregion

        public event Action<GameCompletionResult> OnGameCompletion;

        public void GameCompletion(GameCompletionResult result)
            => OnGameCompletion?.Invoke(result);
    }
}
