using Chess_Console.Others;
using Chess_Console.Data.Enums;
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

        protected override int _maxDistance => 1;

        public override char ChessPieceChar => GameConstants.KnightChar;

        public override bool isCanJumpOverPieces => true;

        public override int PieceValue => 3;

        public KnightPiece(Vector2 piecePosition, ChessSide chessSide) : base(piecePosition, chessSide)
        {

        }

        protected override bool CheckDirectionCycle(Vector2[] directionList, Vector2 targetPosition, ChessAction chessAction)
        {
            Vector2 differDirection = targetPosition - Position;

            if (differDirection.X == 0 && differDirection.Y == 0)
                return false;

            foreach (Vector2 direction in directionList)
            {
                for (int i = 0; i <= _maxDistance; i++)
                {
                    Vector2 targetDestination = new Vector2(direction.X * i, direction.Y * i);

                    if (differDirection == targetDestination)
                        return true;
                }
            }

            return false;
        }
    }
}
