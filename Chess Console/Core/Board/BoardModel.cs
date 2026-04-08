using Chess_Console.Core.Pieces.Base;
using Chess_Console.Core.Common.Enums;
using Chess_Console.Core.Common.Interfaces;
using Chess_Console.Core.Pieces.Instances;

namespace Chess_Console.Core.Board
{
    internal class BoardModel : IServiceInitializable
    {
        private readonly ChessPiece[,] _board = new ChessPiece[_boardHeight, _boardWidth];

        private readonly IPieceFactory _pieceFactory;

        public const int _boardHeight = 8;
        public const int _boardWidth = 8;

        #region Board Initialization

        public BoardModel(IPieceFactory pieceFactory)
        {
            _pieceFactory = pieceFactory;
        }

        public void InitializeService()
        {
            SetupChessSide(ChessSide.Enemy, 0, 1);
            SetupChessSide(ChessSide.Player, _boardHeight - 1, _boardHeight - 2);
        }

        private void SetupChessSide(ChessSide chessSide, int mainRow, int pawnRow)
        {
            GeneratePieces<RookPiece>(chessSide, mainRow, [0, 7]);
            GeneratePieces<KnightPiece>(chessSide, mainRow, [1, 6]);
            GeneratePieces<BishopPiece>(chessSide, mainRow, [2, 5]);

            GeneratePieces<QueenPiece>(chessSide, mainRow, [3]);
            GeneratePieces<KingPiece>(chessSide, mainRow, [4]);

            GeneratePieces<PawnPiece>(chessSide, pawnRow, Enumerable.Range(0, 8).ToArray());
        }

        private void GeneratePieces<T>(ChessSide chessSide, int row, int[] columns) where T : ChessPiece
        {
            foreach (int col in columns)
            {
                Vector2 position = new Vector2(col, row);
                ChessPiece piece = _pieceFactory.CreatePiece<T>(position, chessSide);

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
