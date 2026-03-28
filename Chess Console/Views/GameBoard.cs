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

        private ChessPiece? _enPassantPieceTarget = null;

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

            GeneratePieces(chessSide, pawnRow, Enumerable.Range(0, 8).ToArray(), (pos, s) => new PawnPiece(pos, s));
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

        #region Step Validation

        public bool ValidateMovement(Vector2 startPosition, Vector2 targetPosition, ChessSide chessSide)
        {
            ChessPiece? chessPiece = _board[startPosition.Y, startPosition.X];
            ChessPiece? targetPiece = _board[targetPosition.Y, targetPosition.X];

            if (chessPiece == null)
                return false;

            if (chessPiece.ChessSide != chessSide)
                return false;

            if (targetPiece == null)
            {
                if (chessPiece.CheckMovement(targetPosition)) // check movement
                    return CheckMovementOverPiece(chessPiece, targetPosition, ChessAction.Movement, chessSide);
            }

            else if (chessPiece.CheckBeating(targetPosition) && isEnemyChessSide(chessPiece, targetPosition)) // check beating
                return CheckMovementOverPiece(chessPiece, targetPosition, ChessAction.Beating, chessSide);

            
            if (chessPiece is KingPiece && targetPiece is RookPiece) // check castling
                return MakeCastling(targetPiece as RookPiece, chessSide);

            if (CheckForEnPassant(chessPiece)) // check en passant
                return true;

            return false;
        }


        public bool CheckMovementOverPiece(ChessPiece piece, Vector2 targetPosition, ChessAction chessAction, ChessSide chessSide)
        {
            if (!isPathClear(piece, targetPosition, chessAction))
                return false;

            if (WouldKingBeUnderCheck(piece, targetPosition, chessSide))
            {
                Console.WriteLine("Invalid move: Your king (is / will be) in check!");
                return false;
            }

            return MakeChessPieceStep(piece, targetPosition);
        }

        public bool MakeChessPieceStep(ChessPiece piece, Vector2 targetPosition)
        {
            Vector2 startedPosition = piece.Position;

            MakePossibleSetupEnPassantTarget(piece, startedPosition, targetPosition);

            SetupChessPiece(piece, targetPosition);
            ClearField(startedPosition);

            piece.SetPosition(targetPosition);

            CheckForPawnPromotion(piece);

            return true;
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

        #endregion

        #region Making Step

        // to do

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

        private Vector2 GetAbsoluteDifferenceBetweenVectors(Vector2 firstPosition, Vector2 secondPosition)
        {
            Vector2 difference = firstPosition - secondPosition;
            return new Vector2(Math.Abs(difference.X), Math.Abs(difference.Y));
        }

        private bool isEnemyChessSide(ChessPiece chessPiece, Vector2 targetPosition)
        {
            ChessPiece targetChessPiece = _board[targetPosition.Y, targetPosition.X];
            return targetChessPiece == null ? false : chessPiece.ChessSide != targetChessPiece.ChessSide;
        }

        private IEnumerable<T> GetAllPieceType<T>() where T : ChessPiece
        {
            return _board.Cast<ChessPiece>().OfType<T>();
        }

        #endregion

        private bool WouldKingBeUnderCheck(ChessPiece piece, Vector2 targetPosition, ChessSide chessSide)
        {
            Vector2 originalPosition = piece.Position;
            ChessPiece? targetPieceField = _board[targetPosition.Y, targetPosition.X];

            _board[targetPosition.Y, targetPosition.X] = piece;
            _board[originalPosition.Y, originalPosition.X] = null;

            piece.SetPositionInternal(targetPosition);

            bool isKingUnderCheck = ValidateCheck(chessSide);

            _board[originalPosition.Y, originalPosition.X] = piece;
            _board[targetPosition.Y, targetPosition.X] = targetPieceField;

            piece.SetPositionInternal(originalPosition);

            return isKingUnderCheck;
        }


        private bool CanPieceMoveAnywhere(ChessPiece piece)
        {
            ChessSide side = piece.ChessSide;

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
                        if (!WouldKingBeUnderCheck(piece, target, side))
                            return true;
                    }
                }
            }

            return false;
        }

        #region Check

        public bool ValidateCheck(ChessSide sideUnderAttack)
        {
            KingPiece kingPiece = GetAllPieceType<KingPiece>().FirstOrDefault(k => k.ChessSide == sideUnderAttack);

            ChessSide attackingSide = sideUnderAttack.GetOpposite();

            Vector2 kingPosition = kingPiece.Position;

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

        #endregion

        #region CheckMate

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
                        if (CanPieceMoveAnywhere(piece))
                            return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region Chess Stalemate

        public bool ValidateChessStalemate(ChessSide sideUnderAttack)
        {
            if (ValidateCheck(sideUnderAttack))
                return false;

            for (int y = 0; y < _boardHeight; y++)
            {
                for (int x = 0; x < _boardWidth; x++)
                {
                    ChessPiece piece = _board[y, x];

                    if (piece != null && piece.ChessSide == sideUnderAttack)
                    {
                        if (CanPieceMoveAnywhere(piece))
                            return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region Chess Draw



        #endregion

        #region King Castling

        private bool MakeCastling(RookPiece rook, ChessSide chessSide)
        {
            var kingPiece = GetAllPieceType<KingPiece>().FirstOrDefault(k => k.ChessSide == chessSide);

            int direction = rook.Position.X > kingPiece.Position.X ? 1 : -1;
            int distance = GetAbsoluteDifferenceBetweenVectors(kingPiece.Position, rook.Position).X;

            if (!isPathForCastlingEmpty(kingPiece.Position, rook.Position))
                return false;

            if (isKingCastlingPathUnderAttack(kingPiece.Position, direction, chessSide))
                return false;

            return ExecuteCastling(kingPiece, rook, direction);
        }

        private bool isPathForCastlingEmpty(Vector2 kingPos, Vector2 rookPos)
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

        private bool isKingCastlingPathUnderAttack(Vector2 kingPos, int direction, ChessSide side)
        {
            for (int i = 1; i <= 2; i++)
            {
                Vector2 checkPos = new Vector2(kingPos.X + (i * direction), kingPos.Y);

                if (isFieldUnderAttack(checkPos, side))
                    return true;
            }

            return false;
        }

        public bool isFieldUnderAttack(Vector2 fieldPosition, ChessSide sideUnderAttack)
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

        private bool ExecuteCastling(KingPiece king, RookPiece rook, int direction)
        {
            Vector2 newKingPos = new Vector2(king.Position.X + (2 * direction), king.Position.Y);
            Vector2 newRookPos = new Vector2(newKingPos.X - direction, king.Position.Y);

            MakeChessPieceStep(king, newKingPos);
            MakeChessPieceStep(rook, newRookPos);

            return true;
        }

        #endregion

        #region En Passant Rule

        private void MakePossibleSetupEnPassantTarget(ChessPiece piece, Vector2 startedPosition, Vector2 targetPosition)
        {
            _enPassantPieceTarget = null;

            if (piece is PawnPiece && Math.Abs(targetPosition.Y - startedPosition.Y) == 2)
                _enPassantPieceTarget = piece;
        }

        private bool CheckForEnPassant(ChessPiece myPiece)
        {
            if (myPiece is not PawnPiece || _enPassantPieceTarget == null)
                return false;

            ChessSide chessSide = myPiece.ChessSide;

            Vector2 myPawnPosition = myPiece.Position;
            Vector2 enemyPawnPosition = _enPassantPieceTarget.Position;

            Vector2 difference = GetAbsoluteDifferenceBetweenVectors(myPawnPosition, enemyPawnPosition);

            if (difference.X != 1 || difference.Y != 0)
                return false;

            int myPawnPositionY = chessSide == ChessSide.Player ? myPawnPosition.Y - 1 : myPawnPosition.Y + 1;

            Vector2 targetPosition = new Vector2(enemyPawnPosition.X, myPawnPositionY);

            ChessPiece originalEnemy = _enPassantPieceTarget;

            ClearField(enemyPawnPosition);

            if (WouldKingBeUnderCheck(myPiece, targetPosition, chessSide))
            {
                _board[enemyPawnPosition.Y, enemyPawnPosition.X] = originalEnemy;
                return false;
            }

            return MakeChessPieceStep(myPiece, targetPosition);
        }

        #endregion

        #region Pawn Promotion Rule

        private void CheckForPawnPromotion(ChessPiece piece)
        {
            if (piece is not PawnPiece)
                return;

            int promotionRow = piece.ChessSide == ChessSide.Player ? 0 : _boardHeight - 1;

            if (piece.Position.Y == promotionRow)
            {
                Console.WriteLine("\n--- PAWN PROMOTION! ---");
                Console.WriteLine("Choose a piece: [Q]ueen, [R]ook, [B]ishop, [K]night");

                string input = Console.ReadLine()?.ToUpper() ?? "Q";

                ChessPiece newPiece = input switch
                {
                    "R" => new RookPiece(piece.Position, piece.ChessSide),
                    "B" => new BishopPiece(piece.Position, piece.ChessSide),
                    "K" => new KnightPiece(piece.Position, piece.ChessSide),
                     _  => new QueenPiece(piece.Position, piece.ChessSide)
                };

                _board[piece.Position.Y, piece.Position.X] = newPiece;
            }
        }

        #endregion
    }
}
