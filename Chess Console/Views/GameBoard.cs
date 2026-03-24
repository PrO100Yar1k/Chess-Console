using Chess_Console.Others;
using Chess_Console.Data.Enums;
using Chess_Console.Pieces.Base;
using Chess_Console.Pieces.Instances;

namespace Chess_Console.Views
{
    internal class GameBoard
    {
        private ChessPiece[,] _board = new ChessPiece[_boardHeight, _boardWidth];

        private const int _boardHeight = 8;
        private const int _boardWidth = 8;

        private const int _emptySpacesY = 1;
        private const int _emptySpacesX = 4;

        #region Board Initialization

        public GameBoard()
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
            GeneratePieces<RookPiece>(chessSide, mainRow, new int[] { 0, 7 });
            GeneratePieces<KnightPiece>(chessSide, mainRow, new int[] { 1, 6 });
            GeneratePieces<BishopPiece>(chessSide, mainRow, new int[] { 2, 5 });

            GeneratePieces<QueenPiece>(chessSide, mainRow, new int[] { 3 });
            GeneratePieces<KingPiece>(chessSide, mainRow, new int[] { 4 });

            int[] allColumns = Enumerable.Range(0, _boardWidth).ToArray();
            GeneratePieces<PawnPiece>(chessSide, pawnRow, allColumns);
        }

        private void GeneratePieces<T>(ChessSide side, int row, int[] coloumn) where T : ChessPiece
        {
            foreach (int col in coloumn)
            {
                Vector2 position = new Vector2(col, row);

                T piece = (T) Activator.CreateInstance(typeof(T), position, side);
                SetupChessPiece(piece, position);
            }
        }

        #endregion

        #region Display Board
        
        public void DisplayBoard()
        {
            for (int i = 0; i < _emptySpacesY; i++)
                Console.WriteLine();

            Console.Write(new string(' ', _emptySpacesX + 3));

            for (int i = 0; i < _boardWidth; i++)
            {
                Console.Write($"{i} ");
            }

            Console.WriteLine("\n" + new string(' ', _emptySpacesX + 2) + new string('-', _boardWidth * 2));

            for (int y = 0; y < _boardHeight; y++)
            {
                Console.Write($"{new string(' ', _emptySpacesX)}{y}| ");

                for (int x = 0; x < _boardWidth; x++)
                {
                    ChessPiece chessPiece = _board[y, x];

                    if (chessPiece != null)
                    {
                        string initialSymbolString = chessPiece.ChessPieceChar.ToString();
                        char finalSymbol = chessPiece.ChessSide == ChessSide.Player ? initialSymbolString.ToUpper()[0] : initialSymbolString.ToLower()[0];

                        Console.Write($"{finalSymbol} ");
                    }

                    else Console.Write($"{GameConstants.EmptyChar} ");
                }

                Console.WriteLine();
            }
        }

        #endregion

        #region Movement Validation & Making Step

        public bool ValidateMovement(Vector2 startPosition, Vector2 targetPosition)
        {
            ChessPiece chessPiece = _board[startPosition.Y, startPosition.X];

            if (chessPiece == null)
                return false;

            if (_board[targetPosition.Y, targetPosition.X] == null)
            {
                if (chessPiece.CheckMovement(targetPosition))
                    return CheckMovementOverPiece(chessPiece, targetPosition, ChessAction.Movement);
            }

            else if (chessPiece.CheckBeating(targetPosition) && isEnemyChessSide(chessPiece, targetPosition))
                return CheckMovementOverPiece(chessPiece, targetPosition, ChessAction.Beating);

            return false;
        }


        public bool CheckMovementOverPiece(ChessPiece piece, Vector2 targetPosition, ChessAction chessAction) //
        {
            if (piece.isCanJumpOverPieces)
                return MakeChessPieceStep(piece, targetPosition);

            var path = piece.GetMovementPath(targetPosition, chessAction);

            foreach (var position in path)
            {
                if (_board[position.Y, position.X] != null)
                    return false;
            }

            return MakeChessPieceStep(piece, targetPosition);
        }

        public bool MakeChessPieceStep(ChessPiece piece, Vector2 targetPosition)
        {
            Vector2 startedPosition = piece.PiecePosition;

            SetupChessPiece(piece, targetPosition);
            ClearSquare(startedPosition);

            piece.MakeMovement(targetPosition);

            return true;
        }

        public bool ValidateCheck(ChessSide sideUnderAttack)
        {
            Vector2 kingPosition = FindKingPosition(sideUnderAttack);

            if (kingPosition.X == -1)
                return false;

            ChessSide attackingSide = (sideUnderAttack == ChessSide.Player) ? ChessSide.Enemy : ChessSide.Player;

            for (int y = 0; y < _boardHeight; y++)
            {
                for (int x = 0; x < _boardWidth; x++)
                {
                    ChessPiece piece = _board[y, x];

                    if (piece != null && piece.ChessSide == attackingSide)
                    {
                        if (piece.CheckBeating(kingPosition) && IsPathClear(piece, kingPosition))
                            return true;
                    }
                }
            }

            return false;
        }

        private Vector2 FindKingPosition(ChessSide targetSide)
        {
            for (int y = 0; y < _boardHeight; y++)
            {
                for (int x = 0; x < _boardWidth; x++)
                {
                    var piece = _board[y, x];

                    if (piece is KingPiece && piece.ChessSide == targetSide)
                        return new Vector2(x, y);
                }
            }
            return new Vector2(-1, -1);
        }

        private bool IsPathClear(ChessPiece piece, Vector2 target)
        {
            if (piece.isCanJumpOverPieces)
                return true;

            var path = piece.GetMovementPath(target, ChessAction.Beating);

            foreach (var position in path)
            {
                if (_board[position.Y, position.X] != null)
                    return false;
            }

            return true;
        }

        #endregion

        #region Setup Board

        private void SetupChessPiece(ChessPiece piece, Vector2 position)
        {
            _board[position.Y, position.X] = piece;
        }

        private void ClearSquare(Vector2 position)
        {
            SetupChessPiece(null, position);
        }

        #endregion

        #region Extra Functions

        private bool isEnemyChessSide(ChessPiece chessPiece, Vector2 targetPosition)
        {
            ChessPiece targetChessPiece = _board[targetPosition.Y, targetPosition.X];

            return targetChessPiece == null ? false : chessPiece.ChessSide != targetChessPiece.ChessSide;
        }

        private bool CheckForInPassingRule()
        {
            return false;
        }

        #endregion
    }
}
