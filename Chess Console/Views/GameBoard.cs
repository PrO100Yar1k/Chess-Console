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
            GeneratePieces(chessSide, mainRow, [0, 7], (pos, s) => new RookPiece(pos, s));
            GeneratePieces(chessSide, mainRow, [1, 6], (pos, s) => new KnightPiece(pos, s));
            GeneratePieces(chessSide, mainRow, [2, 5], (pos, s) => new BishopPiece(pos, s));

            GeneratePieces(chessSide, mainRow, [3], (pos, s) => new QueenPiece(pos, s));
            GeneratePieces(chessSide, mainRow, [4], (pos, s) => new KingPiece(pos, s));

            GeneratePieces(chessSide, pawnRow, Enumerable.Range(0, _boardWidth).ToArray(), (pos, s) => new PawnPiece(pos, s));
        }

        private void GeneratePieces<T>(ChessSide side, int row, int[] columns, Func<Vector2, ChessSide, T> factory) where T : ChessPiece
        {
            foreach (int col in columns)
            {
                Vector2 position = new Vector2(col, row);
                T piece = factory(position, side);

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

        public bool ValidateMovement(Vector2 startPosition, Vector2 targetPosition, ChessSide chessSide)
        {
            ChessPiece chessPiece = _board[startPosition.Y, startPosition.X];
            ChessPiece targetPiece = _board[targetPosition.Y, targetPosition.X];

            if (chessPiece == null)
                return false;

            if (chessPiece.ChessSide != chessSide)
                return false;

            if (targetPiece == null)
            {
                if (chessPiece.CheckMovement(targetPosition))
                    return CheckMovementOverPiece(chessPiece, targetPosition, ChessAction.Movement, chessSide);
            }

            else if (chessPiece.CheckBeating(targetPosition) && isEnemyChessSide(chessPiece, targetPosition))
                return CheckMovementOverPiece(chessPiece, targetPosition, ChessAction.Beating, chessSide);

            
            if (chessPiece is KingPiece && targetPiece is RookPiece)
                return MakeCastling(targetPiece as RookPiece, chessSide);

            return false;
        }


        public bool CheckMovementOverPiece(ChessPiece piece, Vector2 targetPosition, ChessAction chessAction, ChessSide chessSide)
        {
            if (!isPathClear(piece, targetPosition, chessAction))
                return false;

            if (isKingUnderCheck(piece, targetPosition, chessSide))
            {
                Console.WriteLine("Invalid move: Your king (is / will be) in check!");
                return false;
            }

            return MakeChessPieceStep(piece, targetPosition);
        }

        public bool MakeChessPieceStep(ChessPiece piece, Vector2 targetPosition)
        {
            Vector2 startedPosition = piece.Position;

            SetupChessPiece(piece, targetPosition);
            ClearField(startedPosition);

            piece.SetPosition(targetPosition);

            return true;
        }

        public bool ValidateCheck(ChessSide sideUnderAttack)
        {
            var kingPositionResult = FindKingPosition(sideUnderAttack);

            if (!kingPositionResult.IsSuccess)
                return false;

            Vector2 kingPosition = kingPositionResult.Value;
            ChessSide attackingSide = sideUnderAttack.GetOpposite();

            for (int y = 0; y < _boardHeight; y++)
            {
                for (int x = 0; x < _boardWidth; x++)
                {
                    ChessPiece piece = _board[y, x];

                    if (piece != null && piece.ChessSide == attackingSide)
                    {
                        if (piece.CheckBeating(kingPosition) && isPathClear(piece, kingPosition, ChessAction.Beating))
                            return true;
                    }
                }
            }

            return false;
        }

        private bool isPathClear(ChessPiece piece, Vector2 targetPosition, ChessAction chessAction)
        {
            if (piece.isCanJumpOverPieces)
                return true;

            var path = piece.GetMovementPath(targetPosition, chessAction);

            foreach (var currentPosition in path)
            {
                if (currentPosition == targetPosition)
                    continue;

                if (_board[currentPosition.Y, currentPosition.X] != null)
                    return false;
            }

            return true;
        }

        private Result<Vector2> FindKingPosition(ChessSide targetSide) //
        {
            for (int y = 0; y < _boardHeight; y++)
            {
                for (int x = 0; x < _boardWidth; x++)
                {
                    var piece = _board[y, x];

                    if (piece is KingPiece && piece.ChessSide == targetSide)
                        return Result<Vector2>.Success(new Vector2(x, y));
                }
            }

            return Result<Vector2>.Failure("The King was not found!");
        }

        #endregion

        #region Setup Board

        private void SetupChessPiece(ChessPiece piece, Vector2 position)
        {
            _board[position.Y, position.X] = piece;
        }

        private void ClearField(Vector2 position)
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

        #endregion

        private bool CheckForEnPassant()
        {
            return false;
        }

        private bool MakeCastling(RookPiece rook, ChessSide chessSide)
        {
            var kingPiece = GetAllPieceType<KingPiece>().FirstOrDefault(k => k.ChessSide == chessSide);

            int direction = rook.Position.X > kingPiece.Position.X ? 1 : -1;
            int distance = GetDistanceFromPieceToPiece(kingPiece, rook);

            if (!IsPathEmpty(kingPiece.Position, rook.Position))
                return false;

            if (IsKingPathUnderAttack(kingPiece.Position, direction, chessSide))
                return false;

            return ExecuteCastling(kingPiece, rook, direction);
        }

        private bool IsPathEmpty(Vector2 kingPos, Vector2 rookPos)
        {
            int startX = Math.Min(kingPos.X, rookPos.X) + 1;
            int endX = Math.Max(kingPos.X, rookPos.X);

            for (int x = startX; x < endX; x++)
            {
                if (_board[kingPos.Y, x] != null)
                    return false;
            }

            return true;
        }

        private bool IsKingPathUnderAttack(Vector2 kingPos, int direction, ChessSide side)
        {
            for (int i = 1; i <= 2; i++)
            {
                Vector2 checkPos = new Vector2(kingPos.X + (i * direction), kingPos.Y);

                if (IsFieldUnderAttack(checkPos, side))
                    return true;
            }

            return false;
        }

        private bool ExecuteCastling(KingPiece king, RookPiece rook, int direction)
        {
            Vector2 newKingPos = new Vector2(king.Position.X + (2 * direction), king.Position.Y);
            Vector2 newRookPos = new Vector2(newKingPos.X - direction, king.Position.Y);

            MakeChessPieceStep(king, newKingPos);
            MakeChessPieceStep(rook, newRookPos);

            return true;
        }

        public bool ValidateCheckmate(ChessSide sideUnderAttack)
        {
            if (!ValidateCheck(sideUnderAttack))
                return false;

            for (int y = 0; y < _boardHeight; y++)
            {
                for (int x = 0; x < _boardWidth; x++)
                {
                    ChessPiece piece = _board[y, x];

                    if (piece != null && piece.ChessSide == sideUnderAttack)
                    {
                        if (CanAnyMoveSaveKing(piece, sideUnderAttack))
                            return false;
                    }
                }
            }

            return true;
        }

        private bool CanAnyMoveSaveKing(ChessPiece piece, ChessSide side)
        {
            for (int y = 0; y < _boardHeight; y++)
            {
                for (int x = 0; x < _boardWidth; x++)
                {
                    Vector2 target = new Vector2(x, y);
                    ChessPiece occupant = _board[y, x];

                    if (occupant != null && occupant.ChessSide == side)
                        continue;

                    bool canPhysicallyMove = occupant == null ? piece.CheckMovement(target) : piece.CheckBeating(target);

                    ChessAction chessAction = occupant == null ? ChessAction.Movement : ChessAction.Beating;

                    if (canPhysicallyMove && isPathClear(piece, target, chessAction))
                    {
                        if (!isKingUnderCheck(piece, target, side))
                            return true;
                    }
                }
            }

            return false;
        }

        private bool isKingUnderCheck(ChessPiece piece, Vector2 targetPosition, ChessSide chessSide)
        {
            Vector2 originalPosition = piece.Position;
            ChessPiece? targetField = _board[targetPosition.Y, targetPosition.X];

            _board[targetPosition.Y, targetPosition.X] = piece;
            _board[originalPosition.Y, originalPosition.X] = null;

            piece.SetPosition(targetPosition);

            bool isKingUnderCheck = ValidateCheck(chessSide);

            _board[originalPosition.Y, originalPosition.X] = piece;
            _board[targetPosition.Y, targetPosition.X] = targetField;
            piece.SetPosition(originalPosition);

            return isKingUnderCheck;
        }

        public bool IsFieldUnderAttack(Vector2 fieldPosition, ChessSide sideUnderAttack)
        {
            ChessSide enemySide = sideUnderAttack.GetOpposite();

            for (int y = 0; y < _boardHeight; y++)
            {
                for (int x = 0; x < _boardWidth; x++)
                {
                    ChessPiece piece = _board[y, x];

                    if (piece != null && piece.ChessSide == enemySide)
                    {
                        if (piece.CheckBeating(fieldPosition) && isPathClear(piece, fieldPosition, ChessAction.Beating))
                            return true;
                    }
                }
            }

            return false;
        }

        private int GetDistanceFromPieceToPiece(ChessPiece chessPiece_1, ChessPiece chessPiece_2)
        {
            Vector2 chessPiecePosition_1 = chessPiece_1.Position;
            Vector2 chessPiecePosition_2 = chessPiece_2.Position;

            int dx = (int) MathF.Abs(chessPiecePosition_2.X - chessPiecePosition_1.X);
            int dy = (int) MathF.Abs(chessPiecePosition_2.Y - chessPiecePosition_1.Y);

            return Math.Max(dx, dy);
        }

        private IEnumerable<T> GetAllPieceType<T>() where T : ChessPiece
        {
            return _board.Cast<ChessPiece>().OfType<T>();
        }
    }
}
