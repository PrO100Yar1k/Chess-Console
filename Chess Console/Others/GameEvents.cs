using Chess_Console.Data.Enums;

namespace Chess_Console.Others
{
    internal sealed class GameEvents
    {
        private static readonly Lazy<GameEvents> _lazy =
                new Lazy<GameEvents>(() => new GameEvents());

        public static GameEvents Instance => _lazy.Value;

        private GameEvents()
        {

        }

        public event Action<ChessSide> OnGameCompletion;

        public void GameCompletion(ChessSide winnerSide)
            => OnGameCompletion?.Invoke(winnerSide);
    }
}
