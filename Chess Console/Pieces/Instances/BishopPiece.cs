using Chess_Console.Others;
using Chess_Console.Pieces.Base;

namespace Chess_Console.Pieces.Instances
{
    internal class BishopPiece : ChessPiece
    {
        protected override Vector2[] _possibleDirectionMoves => new Vector2[] {
            new Vector2(1, 1),
            new Vector2(-1, 1),
            new Vector2(1, -1),
            new Vector2(-1, -1)
        };

        protected override Vector2[] _possibleDirectionBeating => _possibleDirectionMoves;

        public override char ChessPieceChar => GameConstants.BishopChar;

        protected override int _maxDistance => 7;

        public BishopPiece(Vector2 piecePosition, ChessSide chessSide) : base(piecePosition, chessSide)
        {

        }
    }
}
