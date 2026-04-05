using Chess_Console.Core.Pieces.Base;
using Chess_Console.Core.Common.Enums;
using Chess_Console.Infrastructure.Common;

namespace Chess_Console.Core.Pieces.Instances
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

        protected override int _maxDistance => 7;

        public override char ChessPieceChar
            => GameConstants.BishopChar;

        public override int PieceValue => 3;

        public BishopPiece(Vector2 piecePosition, ChessSide chessSide) : base(piecePosition, chessSide)
        {

        }
    }
}
