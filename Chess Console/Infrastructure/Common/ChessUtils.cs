using Chess_Console.Data.Enums;

namespace Chess_Console.Infrastructure.Common
{
    internal static class ChessUtils
    {
        public static ChessSide GetOpposite(this ChessSide side)
        {
            return side == ChessSide.Player ? ChessSide.Enemy : ChessSide.Player;
        }

        // to do
    }
}