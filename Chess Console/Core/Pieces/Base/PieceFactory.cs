using Chess_Console.Core.Pieces.Base;
using Chess_Console.Core.Common.Enums;
using Chess_Console.Core.Pieces.Instances;

internal class PieceFactory : IPieceFactory
{
    private readonly Dictionary<Type, Func<Vector2, ChessSide, ChessPiece>> _creators;

    public PieceFactory()
    {
        _creators = new Dictionary<Type, Func<Vector2, ChessSide, ChessPiece>>
        {
            { typeof(PawnPiece),   (pos, side) => new PawnPiece(pos, side) },
            { typeof(KingPiece),   (pos, side) => new KingPiece(pos, side) },
            { typeof(QueenPiece),  (pos, side) => new QueenPiece(pos, side) },
            { typeof(RookPiece),   (pos, side) => new RookPiece(pos, side) },
            { typeof(BishopPiece), (pos, side) => new BishopPiece(pos, side) },
            { typeof(KnightPiece), (pos, side) => new KnightPiece(pos, side) }
        };
    }

    public T CreatePiece<T>(Vector2 position, ChessSide side) where T : ChessPiece
    {
        if (_creators.TryGetValue(typeof(T), out var creator))
            return (T) creator(position, side);

        throw new NotSupportedException($"Piece {typeof(T).Name} is not registered!");
    }
}