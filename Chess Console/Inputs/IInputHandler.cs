using Chess_Console.Others;

namespace Chess_Console.Inputs
{
    internal interface IInputHandler
    {
        public Result<Move> ParseMove(string input);
        public Result<Vector2> ParsePosition(string input);
    }
}
