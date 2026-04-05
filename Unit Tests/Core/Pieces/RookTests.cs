using Chess_Console.Core.Common.Enums;
using Chess_Console.Core.Pieces.Instances;

public class RookTests
{
    [Theory]
    [InlineData(4, 4, 4, 7)]
    [InlineData(4, 4, 4, 0)]
    [InlineData(4, 4, 0, 4)]
    [InlineData(4, 4, 7, 4)]
    public void RookShouldAllowValidMoves(int x1, int y1, int x2, int y2)
    {
        var rook = new RookPiece(new Vector2(x1, y1), ChessSide.Player);
        Assert.True(rook.CheckMovement(new Vector2(x2, y2)));
    }

    [Theory]
    [InlineData(4, 4, 5, 5)]
    [InlineData(4, 4, 3, 2)]
    [InlineData(4, 4, 6, 5)]
    [InlineData(4, 4, 2, 6)]
    public void RookShouldDenyInvalidMoves(int x1, int y1, int x2, int y2)
    {
        var rook = new RookPiece(new Vector2(x1, y1), ChessSide.Player);
        Assert.False(rook.CheckMovement(new Vector2(x2, y2)));
    }
}