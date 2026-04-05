using Chess_Console.Core.Common.Enums;
using Chess_Console.Core.Pieces.Instances;

public class KnightTests
{
    [Theory]
    [InlineData(4, 4, 6, 5)]
    [InlineData(4, 4, 2, 3)]
    [InlineData(4, 4, 5, 2)]
    [InlineData(4, 4, 3, 6)]
    public void KnightShouldAllowValidMoves(int x1, int y1, int x2, int y2)
    {
        var knight = new KnightPiece(new Vector2(x1, y1), ChessSide.Player);
        Assert.True(knight.CheckMovement(new Vector2(x2, y2)));
    }

    [Theory]
    [InlineData(4, 4, 4, 6)]
    [InlineData(4, 4, 6, 6)]
    [InlineData(4, 4, 4, 2)]
    [InlineData(4, 4, 7, 4)]
    public void KnightShouldDenyInvalidMoves(int x1, int y1, int x2, int y2)
    {
        var knight = new KnightPiece(new Vector2(x1, y1), ChessSide.Player);
        Assert.False(knight.CheckMovement(new Vector2(x2, y2)));
    }
}