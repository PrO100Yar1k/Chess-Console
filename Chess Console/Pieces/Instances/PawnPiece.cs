using Chess_Console.Others;
using Chess_Console.Data.Enums;
using Chess_Console.Pieces.Base;

namespace Chess_Console.Pieces.Instances
{
    internal class PawnPiece : ChessPiece
    {
        protected override Vector2[] _possibleDirectionMoves => _directionMoves;
        protected override Vector2[] _possibleDirectionBeating => _directionBeating;

        private readonly Vector2[] _directionMoves;
        private readonly Vector2[] _directionBeating;

        protected override int _maxDistance => 1;

        public override char ChessPieceChar
            => GameConstants.PawnChar;

        public override int PieceValue => 1;

        public PawnPiece(Vector2 piecePosition, ChessSide chessSide) : base(piecePosition, chessSide)
        {
            int directionY = (chessSide == ChessSide.Player) ? -1 : 1;

            _directionMoves = new Vector2[] { new Vector2(0, directionY) };
            _directionBeating = new Vector2[] { new Vector2(1, directionY), new Vector2(-1, directionY) };
        }

        protected override bool CheckDirectionCycle(Vector2[] directionList, Vector2 targetPosition, ChessAction chessAction)
        {
            if (chessAction == ChessAction.Movement && CountMoves == 0)
            {
                Vector2 doubleStepDestination = new Vector2(_directionMoves[0].X * 2, _directionMoves[0].Y * 2);

                if (targetPosition - Position == doubleStepDestination)
                    return true;
            }

            return base.CheckDirectionCycle(directionList, targetPosition, chessAction);
        }
    }
}
