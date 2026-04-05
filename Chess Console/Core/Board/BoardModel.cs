using Chess_Console.Core.Pieces.Base;
using Chess_Console.Core.Common.Enums;
using Chess_Console.Core.Pieces.Instances;

namespace Chess_Console.Core.Board
{
    internal class BoardModel
    {
        private readonly ChessPiece[,] _board = new ChessPiece[_boardHeight, _boardWidth];

        public const int _boardHeight = 8;
        public const int _boardWidth = 8;

        #region Board Initialization

        public BoardModel()
        {
            InitializeChessPieces();
        }

        private void InitializeChessPieces()
        {
            SetupChessSide(ChessSide.Enemy, 0, 1);
            SetupChessSide(ChessSide.Player, _boardHeight - 1, _boardHeight - 2);
        }

        private void SetupChessSide(ChessSide chessSide, int mainRow, int pawnRow)
        {
            GeneratePieces(chessSide, mainRow, [0, 7], (pos, s) => new RookPiece(pos, s));
            GeneratePieces(chessSide, mainRow, [1, 6], (pos, s) => new KnightPiece(pos, s));
            GeneratePieces(chessSide, mainRow, [2, 5], (pos, s) => new BishopPiece(pos, s));

            GeneratePieces(chessSide, mainRow, [3], (pos, s) => new QueenPiece(pos, s));
            GeneratePieces(chessSide, mainRow, [4], (pos, s) => new KingPiece(pos, s));

            GeneratePieces(chessSide, pawnRow, Enumerable.Range(0, 8).ToArray(), (pos, s) => new PawnPiece(pos, s));
        }

        private void GeneratePieces<T>(ChessSide side, int row, int[] columns, Func<Vector2, ChessSide, T> factory) where T : ChessPiece
        {
            foreach (int col in columns)
            {
                Vector2 position = new Vector2(col, row);
                T piece = factory(position, side);

                SetupChessPiece(position, piece);
            }
        }

        #endregion

        #region Board Interaction

        public ChessPiece GetBoardField(Vector2 position)
        {
            return _board[position.Y, position.X];
        }

        public IEnumerable<T> GetAllPieceType<T>() where T : ChessPiece
        {
            return _board.Cast<ChessPiece>().OfType<T>();
        }

        public void SetupChessPiece(Vector2 position, ChessPiece piece)
        {
            if (!CheckCoordinates(position))
                return;

            _board[position.Y, position.X] = piece;
        }

        public void ClearChessField(Vector2 position)
        {
            if (!CheckCoordinates(position))
                return;

            _board[position.Y, position.X] = null;
        }

        #endregion

        #region Helper Methods

        public bool CheckCoordinates(Vector2 position)
        {
            if (position.X < 0 || position.X >= _boardWidth)
                return false;

            if (position.Y < 0 || position.Y >= _boardHeight)
                return false;

            return true;
        }

        #endregion
    }
}
