using Chess_Console.Core.Common.Enums;
using Chess_Console.Core.Pieces.Instances;

public class QueenTests
{
    [Theory]
    [InlineData(4, 4, 4, 0)]
    [InlineData(4, 4, 0, 4)]
    [InlineData(4, 4, 7, 7)]
    [InlineData(4, 4, 1, 7)]
    public void QueenShouldAllowValidMoves(int x1, int y1, int x2, int y2)
    {
        var queen = new QueenPiece(new Vector2(x1, y1), ChessSide.Player);
        Assert.True(queen.CheckMovement(new Vector2(x2, y2)));
    }

    [Theory]
    [InlineData(4, 4, 5, 6)]
    [InlineData(4, 4, 3, 2)]
    [InlineData(4, 4, 6, 5)]
    [InlineData(4, 4, 2, 5)]
    public void QueenShouldDenyInvalidMoves(int x1, int y1, int x2, int y2)
    {
        var queen = new QueenPiece(new Vector2(x1, y1), ChessSide.Player);
        Assert.False(queen.CheckMovement(new Vector2(x2, y2)));
    }
}