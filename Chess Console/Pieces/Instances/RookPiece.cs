using Chess_Console.Others;
using Chess_Console.Pieces.Base;

namespace Chess_Console.Pieces.Instances
{
    internal class RookPiece : ChessPiece
    {
        public override char ChessPieceChar => GameConstants.RookChar;

        protected override Vector2[] _possibleDirectionMoves => new Vector2[] {
            new Vector2(0, 1),
            new Vector2(0, -1),
            new Vector2(1, 0),  
            new Vector2(-1, 0)
        };

        protected override Vector2[] _possibleDirectionBeating => _possibleDirectionMoves;

        protected override int _maxDistance => 7;

        public RookPiece(Vector2 piecePosition, ChessSide chessSide) : base(piecePosition, chessSide)
        {

        }
    }
}
