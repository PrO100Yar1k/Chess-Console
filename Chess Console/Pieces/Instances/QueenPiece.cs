using Chess_Console.Others;
using Chess_Console.Pieces.Base;

namespace Chess_Console.Pieces.Instances
{
    internal class QueenPiece : ChessPiece
    {
        protected override Vector2[] _possibleDirectionMoves => new Vector2[] {
            new Vector2(0, 1),
            new Vector2(0, -1),
            new Vector2(1, 0),
            new Vector2(-1, 0),

            new Vector2(1, 1),
            new Vector2(-1, 1),
            new Vector2(1, -1),
            new Vector2(-1, -1)
        };

        protected override Vector2[] _possibleDirectionBeating => _possibleDirectionMoves;

        public override char ChessPieceChar => GameConstants.QueenChar;

        protected override int _maxDistance => 7;

        public QueenPiece(Vector2 piecePosition, ChessSide chessSide) : base(piecePosition, chessSide)
        {

        }
    }
}
