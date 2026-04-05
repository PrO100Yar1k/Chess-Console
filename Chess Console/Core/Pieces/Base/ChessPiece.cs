using Chess_Console.Core.Common.Enums;

namespace Chess_Console.Core.Pieces.Base
{
    internal abstract class ChessPiece : IDisposable
    {
        protected abstract Vector2[] _possibleDirectionMoves { get; }
        protected abstract Vector2[] _possibleDirectionBeating { get; }

        protected abstract int _maxDistance { get; }

        public abstract char ChessPieceChar { get; }
        public abstract int PieceValue { get; }

        public readonly ChessSide ChessSide = default;
        public virtual bool isCanJumpOverPieces { get; }

        public Vector2 Position { get; private set; }
        public int CountMoves { get; private set; }

        #region IDisposable

        public void Dispose()
        {

        }

        #endregion

        public ChessPiece(Vector2 piecePosition, ChessSide chessSide)
        {
            Position = piecePosition;
            ChessSide = chessSide;
        }

        public void SetPosition(Vector2 targetPosition)
        {
            Position = targetPosition;
            CountMoves += 1;
        }

        public void SetPositionInternal(Vector2 targetPosition)
        {
            Position = targetPosition;
        }

        public bool CheckMovement(Vector2 targetPosition)
        {
            return CheckDirectionCycle(_possibleDirectionMoves, targetPosition, ChessAction.Movement);
        }

        public bool CheckBeating(Vector2 targetPosition)
        {
            return CheckDirectionCycle(_possibleDirectionBeating, targetPosition, ChessAction.Beating);
        }

        protected virtual bool CheckDirectionCycle(Vector2[] directionList, Vector2 targetPosition, ChessAction chessAction)
        {
            Vector2 diff = targetPosition - Position;

            if (diff.X == 0 && diff.Y == 0)
                return false;

            int stepX = Math.Sign(diff.X);
            int stepY = Math.Sign(diff.Y);

            Vector2 actualDirection = new Vector2(stepX, stepY);

            int actualDistance = Math.Max(Math.Abs(diff.X), Math.Abs(diff.Y));

            if (actualDistance > _maxDistance)
                return false;

            if (diff.X != stepX * actualDistance || diff.Y != stepY * actualDistance)
                return false;

            foreach (Vector2 allowedDir in directionList)
            {
                if (allowedDir.X == stepX && allowedDir.Y == stepY)
                    return true;
            }

            return false;
        }

        public IEnumerable<Vector2> GetMovementPath(Vector2 targetPosition, ChessAction chessAction)
        {
            Vector2 diff = targetPosition - Position;

            int stepX = Math.Sign(diff.X);
            int stepY = Math.Sign(diff.Y);

            int distance = Math.Max(Math.Abs(diff.X), Math.Abs(diff.Y));

            for (int i = 1; i <= distance; i++)
            {
                yield return new Vector2(Position.X + stepX * i, Position.Y + stepY * i);
            }
        }
    }
}
