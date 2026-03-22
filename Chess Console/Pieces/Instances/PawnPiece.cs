using Chess_Console.Data.Enums;
using Chess_Console.Others;
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

        public int CountMoves { get; private set; }

        public override char ChessPieceChar
            => GameConstants.PawnChar;

        public PawnPiece(Vector2 piecePosition, ChessSide chessSide) : base(piecePosition, chessSide)
        {
            int directionY = (chessSide == ChessSide.Player) ? -1 : 1;

            _directionMoves = new Vector2[] { new Vector2(0, directionY) };
            _directionBeating = new Vector2[] { new Vector2(1, directionY), new Vector2(-1, directionY) };
        }

        public override void MakeMovement(Vector2 targetPosition)
        {
            CountMoves += 1;

            base.MakeMovement(targetPosition);
        }

        protected override bool CheckDirectionCycle(Vector2[] directionList, Vector2 targetPosition, ChessAction chessAction)
        {
            if (chessAction == ChessAction.Movement && CountMoves == 0)
            {
                Vector2 doubleStepDestination = new Vector2(_directionMoves[0].X * 2, _directionMoves[0].Y * 2);

                if (targetPosition - PiecePosition == doubleStepDestination)
                    return true;
            }

            return base.CheckDirectionCycle(directionList, targetPosition, chessAction);
        }
    }
}
