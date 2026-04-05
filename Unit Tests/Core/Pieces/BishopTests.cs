using Chess_Console.Core.Common.Enums;
using Chess_Console.Core.Pieces.Instances;

public class BishopTests
{
    [Theory]
    [InlineData(4, 4, 7, 7)]
    [InlineData(4, 4, 1, 1)]
    [InlineData(4, 4, 6, 2)]
    [InlineData(4, 4, 2, 6)]
    public void BishopShouldAllowValidMoves(int x1, int y1, int x2, int y2)
    {
        var bishop = new BishopPiece(new Vector2(x1, y1), ChessSide.Player);
        Assert.True(bishop.CheckMovement(new Vector2(x2, y2)));
    }

    [Theory]
    [InlineData(4, 4, 4, 7)]
    [InlineData(4, 4, 7, 4)]
    [InlineData(4, 4, 4, 1)]
    [InlineData(4, 4, 5, 4)]
    public void BishopShouldDenyInvalidMoves(int x1, int y1, int x2, int y2)
    {
        var bishop = new BishopPiece(new Vector2(x1, y1), ChessSide.Player);
        Assert.False(bishop.CheckMovement(new Vector2(x2, y2)));
    }
}