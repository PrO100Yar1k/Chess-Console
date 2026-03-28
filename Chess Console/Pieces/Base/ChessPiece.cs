using Chess_Console.Data.Enums;

namespace Chess_Console.Pieces.Base
{
    internal abstract class ChessPiece : IDisposable
    {
        protected abstract Vector2[] _possibleDirectionMoves { get; }
        protected abstract Vector2[] _possibleDirectionBeating { get; }

        protected abstract int _maxDistance { get; }
        public abstract char ChessPieceChar { get; }

        public virtual bool isCanJumpOverPieces { get; } = false;

        public readonly ChessSide ChessSide = default;

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

        public virtual bool CheckMovement(Vector2 targetPosition)
        {
            return CheckDirectionCycle(_possibleDirectionMoves, targetPosition, ChessAction.Movement);
        }

        public virtual bool CheckBeating(Vector2 targetPosition)
        {
            return CheckDirectionCycle(_possibleDirectionBeating, targetPosition, ChessAction.Beating);
        }

        protected virtual bool CheckDirectionCycle(Vector2[] directionList, Vector2 targetPosition, ChessAction chessAction)
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

        public IEnumerable<Vector2> GetMovementPath(Vector2 targetPosition, ChessAction chessAction)
        {
            Vector2[] directionList = chessAction == ChessAction.Movement ? _possibleDirectionMoves : _possibleDirectionBeating;

            Vector2 differDirection = targetPosition - Position;

            foreach (Vector2 direction in directionList)
            {
                for (int i = 1; i <= _maxDistance; i++)
                {
                    Vector2 currentStep = new Vector2(direction.X * i, direction.Y * i);

                    if (differDirection == currentStep)
                    {
                        for (int j = 1; j <= i; j++)
                        {
                            yield return Position + new Vector2(direction.X * j, direction.Y * j);
                        }

                        yield break;
                    }
                }
            }
        }
    }
}
