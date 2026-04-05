using Chess_Console.Core.Common.Enums;
using Chess_Console.Core.Pieces.Instances;

public class KingTests
{
    [Theory]
    [InlineData(4, 4, 4, 5)]
    [InlineData(4, 4, 5, 5)]
    [InlineData(4, 4, 3, 4)]
    [InlineData(4, 4, 3, 3)]
    public void KingShouldAllowValidMoves(int x1, int y1, int x2, int y2)
    {
        var king = new KingPiece(new Vector2(x1, y1), ChessSide.Player);
        Assert.True(king.CheckMovement(new Vector2(x2, y2)));
    }

    [Theory]
    [InlineData(4, 4, 4, 6)]
    [InlineData(4, 4, 6, 6)]
    [InlineData(4, 4, 2, 4)]
    [InlineData(4, 4, 1, 1)]
    public void KingShouldDenyInvalidMoves(int x1, int y1, int x2, int y2)
    {
        var king = new KingPiece(new Vector2(x1, y1), ChessSide.Player);
        Assert.False(king.CheckMovement(new Vector2(x2, y2)));
    }
}