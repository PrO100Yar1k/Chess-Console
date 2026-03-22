using Chess_Console.Data.Enums;
using Chess_Console.Others;
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

        public GameBoard()
        {
            InitializeChessPieces();
        }

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
            
        public bool CheckMovementOverPiece(ChessPiece piece, Vector2 targetPosition, ChessAction chessAction)
        {
            if (piece.isJumpOverPieces)
                return MakeChessPieceStep(piece, targetPosition);

            var path = piece.GetMovementPath(targetPosition, chessAction);

            foreach (var position in path)
            {
                if (_board[position.Y, position.X] != null)
                {
                    Console.WriteLine("hueta");
                    return false;
                }
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

        private void InitializeChessPieces()
        {
            SetupChessSide(ChessSide.Enemy, 0, 1);
            SetupChessSide(ChessSide.Player, _boardHeight - 1, _boardHeight - 2);
        }

        private void SetupChessSide(ChessSide chessSide, int mainColoumn, int pawnRow)
        {
            GenerateRooks(chessSide, mainColoumn);
            GenerateKnights(chessSide, mainColoumn);
            GenerateBishops(chessSide, mainColoumn);
            GenerateQueen(chessSide, mainColoumn);
            GenerateKing(chessSide, mainColoumn);

            GeneratePawns(chessSide, pawnRow);
        }

        private void GeneratePawns(ChessSide chessSide, int coloumn)
        {
            for (int i = 0; i < _boardWidth; i++)
            {
                Vector2 position = new Vector2(i, coloumn);

                PawnPiece chessPiece = new PawnPiece(position, chessSide);
                SetupChessPiece(chessPiece, position);
            }
        }

        private void GenerateRooks(ChessSide chessSide, int coloumn) // to do
        {
            int[] rookRows = { 0, 7 };

            foreach (int row in rookRows)
            {
                Vector2 position = new Vector2(row, coloumn);

                RookPiece chessPiece = new RookPiece(position, chessSide);
                SetupChessPiece(chessPiece, position);
            }
        }

        private void GenerateKnights(ChessSide chessSide, int coloumn)
        {
            int[] knightRows = { 1, 6 };

            foreach (int row in knightRows)
            {
                Vector2 position = new Vector2(row, coloumn);

                KnightPiece chessPiece = new KnightPiece(position, chessSide);
                SetupChessPiece(chessPiece, position);
            }
        }

        private void GenerateBishops(ChessSide chessSide, int coloumn)
        {
            int[] bishopRows = { 2, 5 };

            foreach (int row in bishopRows)
            {
                Vector2 spawnPosition = new Vector2(row, coloumn);

                BishopPiece chessPiece = new BishopPiece(spawnPosition, chessSide);
                SetupChessPiece(chessPiece, spawnPosition);
            }
        }

        private void GenerateQueen(ChessSide chessSide, int coloumn)
        {
            Vector2 spawnPosition = new Vector2(3, coloumn);

            QueenPiece chessPiece = new QueenPiece(spawnPosition, chessSide);
            SetupChessPiece(chessPiece, spawnPosition);
        }

        private void GenerateKing(ChessSide chessSide, int coloumn)
        {
            Vector2 spawnPosition = new Vector2(4, coloumn);

            KingPiece chessPiece = new KingPiece(spawnPosition, chessSide);
            SetupChessPiece(chessPiece, spawnPosition);
        }



        private void SetupChessPiece(ChessPiece piece, Vector2 position)
        {
            _board[position.Y, position.X] = piece;
        }

        private void ClearSquare(Vector2 position)
        {
            SetupChessPiece(null, position);
        }


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

        private bool isEnemyChessSide(ChessPiece chessPiece, Vector2 targetPosition)
        {
            ChessPiece targetChessPiece = _board[targetPosition.Y, targetPosition.X];

            return chessPiece.ChessSide != targetChessPiece.ChessSide;
        }

        private bool CheckForInPassingRule()
        {
            return false;
        }
    }
}
