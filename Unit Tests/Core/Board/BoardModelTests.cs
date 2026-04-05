using Chess_Console.Core.Board;
using Chess_Console.Core.Common.Enums;
using Chess_Console.Core.Pieces.Base;
using Chess_Console.Core.Pieces.Instances;

public class BoardModelTests
{
    [Fact]
    public void InitializeShouldPlacePiecesInCorrectPositions()
    {
        var board = new BoardModel();

        ChessPiece whitePawn = board.GetBoardField(new Vector2(4, 6));
        ChessPiece blackPawn = board.GetBoardField(new Vector2(4, 1));

        Assert.NotNull(whitePawn);
        Assert.Equal(ChessSide.Player, whitePawn.ChessSide);
        Assert.NotNull(blackPawn);
        Assert.Equal(ChessSide.Enemy, blackPawn.ChessSide);
    }

    [Fact]
    public void SetPieceShouldUpdateBoardState()
    {
        var board = new BoardModel();
        Vector2 position = new Vector2(4, 4);

        RookPiece rook = new RookPiece(position, ChessSide.Player);
        board.SetupChessPiece(position, rook);

        Assert.Equal(rook, board.GetBoardField(position));
    }

    [Fact]
    public void RemovePieceShouldLeaveEmptyField()
    {
        var board = new BoardModel();
        Vector2 position = new Vector2(3, 3);

        board.SetupChessPiece(position, new KnightPiece(position, ChessSide.Player));
        board.ClearChessField(position);

        Assert.Null(board.GetBoardField(position));
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(8, 0)]
    [InlineData(0, -1)]
    [InlineData(0, 8)]
    public void IsWithinBoundsShouldReturnFalseForInvalidCoordinates(int x, int y)
    {
        var board = new BoardModel();
        bool isInside = board.CheckCoordinates(new Vector2(x, y));

        Assert.False(isInside);
    }

    //[Fact]
    //public void MovePieceShouldUpdatePositionAndClearOldCell()
}