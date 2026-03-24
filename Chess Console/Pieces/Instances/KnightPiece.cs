using Chess_Console.Data.Enums;
using Chess_Console.Others;
using Chess_Console.Pieces.Base;

namespace Chess_Console.Pieces.Instances
{
    internal class KnightPiece : ChessPiece
    {
        protected override Vector2[] _possibleDirectionMoves => new Vector2[] {
            new Vector2(1, 2),
            new Vector2(-1, 2),
            new Vector2(1, -2),
            new Vector2(-1, -2),

            new Vector2(2, 1),
            new Vector2(2, -1),
            new Vector2(-2, 1),
            new Vector2(-2, -1),
        };

        protected override Vector2[] _possibleDirectionBeating => _possibleDirectionMoves;

        public override char ChessPieceChar => GameConstants.KnightChar;

        protected override int _maxDistance => 1;

        public override bool isCanJumpOverPieces { get; } = true;

        public KnightPiece(Vector2 piecePosition, ChessSide chessSide) : base(piecePosition, chessSide)
        {

        }
    }
}
