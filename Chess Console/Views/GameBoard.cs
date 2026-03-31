using System.Text;
using Chess_Console.Others;
using Chess_Console.Data.Enums;
using Chess_Console.Pieces.Base;
using Chess_Console.Pieces.Instances;

namespace Chess_Console.Views
{
    internal class GameBoard
    {
        private readonly ChessPiece[,] _board = new ChessPiece[_boardHeight, _boardWidth];

        private readonly Dictionary<string, int> _positionHistory = new();

        private PawnPiece? _enPassantPieceTarget = default;

        private const int _boardHeight = 8;
        private const int _boardWidth = 8;

        private const int _emptySpacesY = 1;
        private const int _emptySpacesX = 4;

        private int _halfMoveCounter = 0;

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
            //GeneratePieces(chessSide, mainRow, [0, 7], (pos, s) => new RookPiece(pos, s));
            GeneratePieces(chessSide, mainRow, [1, 6], (pos, s) => new KnightPiece(pos, s));
            //GeneratePieces(chessSide, mainRow, [2, 5], (pos, s) => new BishopPiece(pos, s));

            //GeneratePieces(chessSide, mainRow, [3], (pos, s) => new QueenPiece(pos, s));
            GeneratePieces(chessSide, mainRow, [4], (pos, s) => new KingPiece(pos, s));

            //GeneratePieces(chessSide, pawnRow, Enumerable.Range(0, 8).ToArray(), (pos, s) => new PawnPiece(pos, s));
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

            for (int y = 0; y < _boardHeight; y++)
            {
                int rowNumber = _boardHeight - y;

                Console.Write($"{new string(' ', _emptySpacesX)}{rowNumber}| ");

                for (int x = 0; x < _boardWidth; x++)
                {
                    ChessPiece chessPiece = _board[y, x];

                    if (chessPiece != null)
                    {
                        string initialSymbolString = chessPiece.ChessPieceChar.ToString();

                        char finalSymbol = chessPiece.ChessSide == ChessSide.Player
                            ? initialSymbolString.ToUpper()[0]
                            : initialSymbolString.ToLower()[0];

                        Console.Write($"{finalSymbol} ");
                    }
                    else
                    {
                        Console.Write($"{GameConstants.EmptyChar} ");
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine(new string(' ', _emptySpacesX + 2) + new string('-', _boardWidth * 2));
            Console.Write(new string(' ', _emptySpacesX + 3));

            for (int i = 0; i < _boardWidth; i++)
            {
                Console.Write($"{(char)('a' + i)} ");
            }

            Console.WriteLine();
        }

        #endregion

        #region Step Validation

        public Result<Action> ValidateMovement(Vector2 startPosition, Vector2 targetPosition, ChessSide chessSide)
        {
            ChessPiece? chessPiece = _board[startPosition.Y, startPosition.X];
            ChessPiece? targetPiece = _board[targetPosition.Y, targetPosition.X];

            if (chessPiece == null)
                return Result<Action>.Failure("Start field is empty!");

            if (chessPiece.ChessSide != chessSide)
                return Result<Action>.Failure("You cannot use enemy piece!");

            if (targetPiece == null)
            {
                if (chessPiece.CheckMovement(targetPosition))
                    return CheckMovementOverPiece(chessPiece, targetPosition, ChessAction.Movement, chessSide);

                if (ValidateEnPassant(chessPiece, targetPosition))
                    return ExecuteEnPassant(chessPiece);
            }

            else if (chessPiece.CheckBeating(targetPosition) && isEnemyChessSide(chessPiece, targetPiece))
                return CheckMovementOverPiece(chessPiece, targetPosition, ChessAction.Beating, chessSide);

            else if (CheckForCastling(chessPiece, targetPiece))
                return MakeCastling(chessPiece as KingPiece, targetPiece as RookPiece, chessSide);

            return Result<Action>.Failure("Invalid input, please try again!");
        }

        private Result<Action> CheckMovementOverPiece(ChessPiece piece, Vector2 targetPosition, ChessAction chessAction, ChessSide chessSide)
        {
            if (!isPathClear(piece, targetPosition, chessAction))
                return Result<Action>.Failure("You cannot jump over other pieces!");

            if (WouldKingBeUnderCheck(piece, targetPosition, chessSide))
                return Result<Action>.Failure("Invalid move: Your king (is / will be) in check!");

            return Result<Action>.Success(() => MakeChessPieceStep(piece, targetPosition));
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

        private void MakeChessPieceStep(ChessPiece piece, Vector2 targetPosition)
        {
            Vector2 startedPosition = piece.Position;

            ConfigureEnPassantTarget(piece, startedPosition, targetPosition);

            UpdateFiftyMoveCounter(piece, _board[targetPosition.Y, targetPosition.X] != null);

            SetupChessPiece(piece, targetPosition);
            ClearChessField(startedPosition);

            piece.SetPosition(targetPosition);

            CheckForPawnPromotion(piece);

            RecordPosition(piece.ChessSide);
        }

        #endregion

        #region Board Interaction

        private void SetupChessPiece(ChessPiece piece, Vector2 position)
        {
            _board[position.Y, position.X] = piece;
        }

        private void ClearChessField(Vector2 position)
        {
            SetupChessPiece(null, position);
        }

        #endregion

        #region Helper Methods

        private Vector2 GetAbsoluteDifferenceBetweenVectors(Vector2 firstPosition, Vector2 secondPosition)
        {
            Vector2 difference = firstPosition - secondPosition;
            return new Vector2(Math.Abs(difference.X), Math.Abs(difference.Y));
        }

        private bool isEnemyChessSide(ChessPiece chessPiece, ChessPiece targetChessPiece)
        {
            return targetChessPiece != null && chessPiece.ChessSide != targetChessPiece.ChessSide;
        }

        private IEnumerable<T> GetAllPieceType<T>() where T : ChessPiece
        {
            return _board.Cast<ChessPiece>().OfType<T>();
        }

        #endregion

        #region Check Any Piece Movement

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

        #endregion

        #region Simulating

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

        #endregion

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

        #region Repeat Steps Draw

        public bool ValidateRepeatStepsDraw()
        {
            return _positionHistory.Values.Any(count => count >= 3);
        }

        public void RecordPosition(ChessSide chessSide)
        {
            string snapshot = GetPositionSnapshot(chessSide);

            if (_positionHistory.ContainsKey(snapshot))
                _positionHistory[snapshot] += 1;
            else
                _positionHistory[snapshot] = 1;
        }

        private string GetPositionSnapshot(ChessSide chessSide)
        {
            StringBuilder sb = new StringBuilder();

            for (int y = 0; y < _boardHeight; y++)
            {
                for (int x = 0; x < _boardWidth; x++)
                {
                    var piece = _board[y, x];
                    sb.Append(piece == null ? "." : piece.ChessPieceChar.ToString() + (int)piece.ChessSide);
                }
            }

            sb.Append((int)chessSide);
            sb.Append(_enPassantPieceTarget != null ? _enPassantPieceTarget.Position.ToString() : "none");

            return sb.ToString();
        }

        #endregion

        #region Fifty Steps No Beating Draw

        public void UpdateFiftyMoveCounter(ChessPiece movingPiece, bool wasCapturing)
        {
            if (movingPiece is PawnPiece || wasCapturing)
                _halfMoveCounter = 0;

            else _halfMoveCounter++;
        }

        public bool ValidateFiftyStepsNoBeatingDraw()
        {
            return _halfMoveCounter >= 100;
        }

        #endregion

        #region King Castling

        private bool CheckForCastling(ChessPiece chessPiece, ChessPiece targetPiece)
        {
            return chessPiece is KingPiece && targetPiece is RookPiece && chessPiece.ChessSide == targetPiece.ChessSide;
        }

        private Result<Action> MakeCastling(KingPiece king, RookPiece rook, ChessSide chessSide)
        {
            int direction = rook.Position.X > king.Position.X ? 1 : -1;
            int distance = GetAbsoluteDifferenceBetweenVectors(king.Position, rook.Position).X;

            if (!isPathBetweenEmpty(king.Position, rook.Position))
                return Result<Action>.Failure("Path between king and rook is not empty!");

            if (isKingCastlingPathUnderAttack(king.Position, direction, chessSide))
                return Result<Action>.Failure("King path is under attack!");

            Vector2 newKingPos = new Vector2(king.Position.X + (2 * direction), king.Position.Y);
            Vector2 newRookPos = new Vector2(newKingPos.X - direction, king.Position.Y);

            return Result<Action>.Success(() =>
            {
                MakeChessPieceStep(king, newKingPos);
                MakeChessPieceStep(rook, newRookPos);
            });
        }

        private bool isPathBetweenEmpty(Vector2 kingPos, Vector2 rookPos)
        {
            int startX = Math.Min(kingPos.X, rookPos.X) + 1;
            int endX = Math.Max(kingPos.X, rookPos.X);

            for (int X = startX; X < endX; X++)
            {
                if (_board[kingPos.Y, X] != null)
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

        private bool isFieldUnderAttack(Vector2 fieldPosition, ChessSide sideUnderAttack)
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

        #endregion

        #region En Passant Rule

        private void ConfigureEnPassantTarget(ChessPiece piece, Vector2 startedPosition, Vector2 targetPosition)
        {
            _enPassantPieceTarget = null;

            if (piece is PawnPiece && Math.Abs(targetPosition.Y - startedPosition.Y) == 2)
                _enPassantPieceTarget = piece as PawnPiece;
        }

        private bool ValidateEnPassant(ChessPiece myPiece, Vector2 targetPosition)
        {
            if (myPiece is not PawnPiece || _enPassantPieceTarget == null)
                return false;

            Vector2 myPawnPosition = myPiece.Position;
            Vector2 enemyPawnPosition = _enPassantPieceTarget.Position;

            Vector2 difference = GetAbsoluteDifferenceBetweenVectors(myPawnPosition, enemyPawnPosition);

            if (difference.X != 1 || difference.Y != 0)
                return false;

            return true;
        }

        private Result<Action> ExecuteEnPassant(ChessPiece myPiece)
        {
            ChessSide chessSide = myPiece.ChessSide;

            Vector2 myPawnPosition = myPiece.Position;
            Vector2 enemyPawnPosition = _enPassantPieceTarget.Position;

            int myPawnPositionY = chessSide == ChessSide.Player ? myPawnPosition.Y - 1 : myPawnPosition.Y + 1;

            Vector2 targetPosition = new Vector2(enemyPawnPosition.X, myPawnPositionY);

            ChessPiece originalEnemy = _enPassantPieceTarget;

            ClearChessField(enemyPawnPosition);

            bool isKingUnderCheck = WouldKingBeUnderCheck(myPiece, targetPosition, chessSide);

            _board[enemyPawnPosition.Y, enemyPawnPosition.X] = originalEnemy;

            if (isKingUnderCheck == true)
                return Result<Action>.Failure("Your king would be in check!");

            return Result<Action>.Success(() =>
            {
                ClearChessField(enemyPawnPosition);
                MakeChessPieceStep(myPiece, targetPosition);
            });
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
                    _ => new QueenPiece(piece.Position, piece.ChessSide)
                };

                _board[piece.Position.Y, piece.Position.X] = newPiece;
            }
        }

        #endregion

        #region Insufficient Material

        public bool ValidateInsufficientMaterial()
        {
            var activePieces = GetActivePieces().ToList();

            if (activePieces.Any(p => p is PawnPiece or RookPiece or QueenPiece))
                return false;

            int whiteMaterial = activePieces.Where(p => p.ChessSide == ChessSide.Player).Sum(p => p.PieceValue);
            int blackMaterial = activePieces.Where(p => p.ChessSide == ChessSide.Enemy).Sum(p => p.PieceValue);

            if (whiteMaterial <= 3 && blackMaterial <= 3)
            {
                if (whiteMaterial == 3 && blackMaterial == 3)
                {
                    var bishops = activePieces.OfType<BishopPiece>().ToList();

                    if (bishops.Count == 2)
                        return AreBishopsOnSameColor(bishops);

                    return true;
                }

                return true;
            }

            return false;
        }

        private bool AreBishopsOnSameColor(List<BishopPiece> bishops)
        {
            Vector2 pos1 = bishops[0].Position;
            Vector2 pos2 = bishops[1].Position;

            return (pos1.X + pos1.Y) % 2 == (pos2.X + pos2.Y) % 2;
        }

        private IEnumerable<ChessPiece> GetActivePieces()
        {
            for (int y = 0; y < _boardHeight; y++)
            {
                for (int x = 0; x < _boardWidth; x++)
                {
                    ChessPiece currentPiece = _board[y, x];

                    if (currentPiece != null)
                        yield return currentPiece;
                }
            }
        }

        #endregion
    }
}
