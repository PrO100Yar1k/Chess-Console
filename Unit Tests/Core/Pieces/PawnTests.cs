using Chess_Console.Core.Common.Enums;
using Chess_Console.Core.Pieces.Instances;

public class PawnTests
{
    [Theory]
    [InlineData(4, 6, 4, 5)]
    [InlineData(3, 6, 3, 4)]
    [InlineData(2, 6, 2, 4)]
    [InlineData(1, 5, 1, 3)]
    public void PlayerPawnShouldAllowValidMoves(int x1, int y1, int x2, int y2)
    {
        var pawn = new PawnPiece(new Vector2(x1, y1), ChessSide.Player);
        Assert.True(pawn.CheckMovement(new Vector2(x2, y2)));
    }

    [Theory]
    [InlineData(4, 6, 4, 7)]
    [InlineData(4, 1, 4, 2)]
    [InlineData(4, 4, 6, 0)]
    [InlineData(2, 6, 3, 5)]
    public void PlayerPawnShouldDenyInvalidMoves(int x1, int y1, int x2, int y2)
    {
        var pawn = new PawnPiece(new Vector2(x1, y1), ChessSide.Player);
        Assert.False(pawn.CheckMovement(new Vector2(x2, y2)));
    }

    [Theory]
    [InlineData(2, 1, 2, 3)]
    [InlineData(4, 1, 4, 3)]
    [InlineData(6, 6, 6, 8)]
    [InlineData(3, 5, 3, 6)]
    public void EnemyPawnShouldAllowValidMoves(int x1, int y1, int x2, int y2)
    {
        var pawn = new PawnPiece(new Vector2(x1, y1), ChessSide.Enemy);
        Assert.True(pawn.CheckMovement(new Vector2(x2, y2)));
    }

    [Theory]
    [InlineData(2, 3, 2, 2)]
    [InlineData(4, 5, 4, 2)]
    [InlineData(6, 7, 6, 6)]
    [InlineData(3, 4, 3, 0)]
    public void EnemyPawnShouldDenyInvalidMoves(int x1, int y1, int x2, int y2)
    {
        var pawn = new PawnPiece(new Vector2(x1, y1), ChessSide.Enemy);
        Assert.False(pawn.CheckMovement(new Vector2(x2, y2)));
    }
}