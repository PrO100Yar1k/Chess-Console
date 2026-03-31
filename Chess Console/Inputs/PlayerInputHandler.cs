using Chess_Console.Others;

namespace Chess_Console.Inputs
{
    internal class PlayerInputHandler : IInputHandler
    {
        public Result<Move> ParseMove(string input)
        {
            if (string.IsNullOrEmpty(input))
                return Result<Move>.Failure("Input is empty");

            input = input.Trim().Replace(" ", "").ToLower();

            if (input.Length != 4)
                return Result<Move>.Failure("Input must match with example input (e2 e4)");

            string fromStr = input.Substring(0, 2);
            string toStr = input.Substring(2, 2);

            var fromResult = ParsePosition(fromStr);
            var toResult = ParsePosition(toStr);

            if (!fromResult.IsSuccess)
                return Result<Move>.Failure($"From: {fromResult.Error}");

            if (!toResult.IsSuccess)
                return Result<Move>.Failure($"To: {toResult.Error}");

            return Result<Move>.Success(new Move(fromResult.Value, toResult.Value));
        }

        public Result<Vector2> ParsePosition(string input)
        {
            char file = input[0];
            char rank = input[1];

            if (file < 'a' || file > 'h' || rank < '1' || rank > '8')
                return Result<Vector2>.Failure("Position out of board range");

            int row = file - 'a';
            int col = 8 - (rank - '0');

            return Result<Vector2>.Success(new Vector2(row, col));
        }
    }
}