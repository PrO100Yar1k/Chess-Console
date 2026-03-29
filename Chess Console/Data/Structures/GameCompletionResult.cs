using Chess_Console.Data.Enums;

namespace Chess_Console.Data.Structures
{
    internal struct GameCompletionResult
    {
        public string Message { get; }
        public GameCompletionType Type { get; }
        public ChessSide? WinnerSide { get; }

        public GameCompletionResult(string message, GameCompletionType type, ChessSide? winnerSide = null)
        {
            Message = message;
            Type = type;
            WinnerSide = winnerSide;
        }
    }
}