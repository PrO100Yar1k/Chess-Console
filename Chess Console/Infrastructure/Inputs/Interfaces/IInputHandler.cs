using Chess_Console.Infrastructure.Common;

namespace Chess_Console.Infrastructure.Inputs.Interfaces
{
    internal interface IInputHandler
    {
        public Result<Move> ParseMove(string input);
        public Result<Vector2> ParsePosition(string input);
    }
}
