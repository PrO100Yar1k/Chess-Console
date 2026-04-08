using Chess_Console.Core.Common.Enums;
using Chess_Console.Core.Pieces.Base;

internal interface IPieceFactory
{
    public T CreatePiece<T>(Vector2 position, ChessSide side) where T : ChessPiece;
}