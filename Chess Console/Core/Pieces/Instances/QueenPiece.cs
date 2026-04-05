using Chess_Console.Data.Enums;
using Chess_Console.Core.Pieces.Base;
using Chess_Console.Infrastructure.Common;

namespace Chess_Console.Core.Pieces.Instances
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

        protected override int _maxDistance => 7;

        public override char ChessPieceChar
            => GameConstants.QueenChar;

        public override int PieceValue => 9;

        public QueenPiece(Vector2 piecePosition, ChessSide chessSide) : base(piecePosition, chessSide)
        {

        }
    }
}
