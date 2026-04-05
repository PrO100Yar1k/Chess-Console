using Chess_Console.Data.Enums;
using Chess_Console.Pieces.Instances;

namespace Chess_Console.Tests
{
    public class RookPieceTests
    {
        [Theory]
        [InlineData(4, 4, 4, 7)]
        [InlineData(4, 4, 4, 0)]
        [InlineData(4, 4, 0, 4)]
        [InlineData(4, 4, 7, 4)]
        public void RookShouldAllowVerticalAndHorizontalMoves(int startX, int startY, int targetX, int targetY)
        {
            var startPos = new Vector2(startX, startY);
            var targetPos = new Vector2(targetX, targetY);
            var rook = new RookPiece(startPos, ChessSide.Player);

            bool canMove = rook.CheckMovement(targetPos);

            Assert.True(canMove, $"Rook should be able to move from {startPos} to {targetPos}");
        }

        [Theory]
        [InlineData(4, 4, 5, 5)]
        [InlineData(4, 4, 2, 6)]
        [InlineData(4, 4, 5, 6)]
        public void RookShouldDenyInvalidMoves(int startX, int startY, int targetX, int targetY)
        {
            var startPos = new Vector2(startX, startY);
            var targetPos = new Vector2(targetX, targetY);
            var rook = new RookPiece(startPos, ChessSide.Player);

            bool canMove = rook.CheckMovement(targetPos);

            Assert.False(canMove, $"Rook should NOT be able to move from {startPos} to {targetPos}");
        }
    }
}