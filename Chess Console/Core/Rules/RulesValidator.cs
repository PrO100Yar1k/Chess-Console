using System.Text;
using Chess_Console.Views;
using Chess_Console.Data.Enums;
using Chess_Console.Controllers;
using Chess_Console.Core.Pieces.Base;
using Chess_Console.Core.Pieces.Instances;
using Chess_Console.Infrastructure.Common;
using Chess_Console.Core.Board;

namespace Chess_Console.Core.Rules
{
    internal class RulesValidator
    {
        private readonly Dictionary<string, int> _positionHistory = new();

        private PawnPiece? _enPassantPieceTarget;

        private readonly BoardModel _boardModel;
        private StepController _stepController;

        private int _halfMoveCounter = 0;

        public RulesValidator(BoardModel boardModel)
        {
            _boardModel = boardModel;
        }

        public void SetStepController(StepController stepController)
        {
            _stepController = stepController;
        }

        #region Check

        public bool ValidateCheck(ChessSide sideUnderAttack)
        {
            KingPiece kingPiece = _boardModel.GetAllPieceType<KingPiece>().FirstOrDefault(k => k.ChessSide == sideUnderAttack);

            ChessSide attackingSide = sideUnderAttack.GetOpposite();

            Vector2 kingPosition = kingPiece.Position;

            for (int y = 0; y < BoardModel._boardHeight; y++)
            {
                for (int x = 0; x < BoardModel._boardWidth; x++)
                {
                    Vector2 targetPosition = new Vector2(x, y);
                    ChessPiece piece = _boardModel.GetBoardField(targetPosition);

                    if (piece != null && piece.ChessSide == attackingSide)
                    {
                        if (piece.CheckBeating(kingPosition) && isPathClear(piece, kingPosition, ChessAction.Beating))
                            return true;
                    }
                }
            }

            return false;
        }

        public bool isPathClear(ChessPiece piece, Vector2 targetPosition, ChessAction chessAction)
        {
            if (piece.isCanJumpOverPieces)
                return true;

            var path = piece.GetMovementPath(targetPosition, chessAction);

            foreach (var currentPosition in path)
            {
                if (currentPosition == targetPosition)
                    continue;

                if (_boardModel.GetBoardField(currentPosition) != null)
                    return false;
            }

            return true;
        }

        #endregion

        #region Checkmate

        public bool ValidateCheckmate(ChessSide sideUnderAttack)
        {
            if (!ValidateCheck(sideUnderAttack))
                return false;

            for (int y = 0; y < BoardModel._boardHeight; y++)
            {
                for (int x = 0; x < BoardModel._boardWidth; x++)
                {
                    Vector2 targetPosition = new Vector2(x, y);
                    ChessPiece piece = _boardModel.GetBoardField(targetPosition);

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

        #region Stalemate

        public bool ValidateStalemate(ChessSide sideUnderAttack)
        {
            if (ValidateCheck(sideUnderAttack))
                return false;

            for (int y = 0; y < BoardModel._boardHeight; y++)
            {
                for (int x = 0; x < BoardModel._boardWidth; x++)
                {
                    Vector2 targetPosition = new Vector2(x, y);
                    ChessPiece piece = _boardModel.GetBoardField(targetPosition);

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

            for (int y = 0; y < BoardModel._boardHeight; y++)
            {
                for (int x = 0; x < BoardModel._boardWidth; x++)
                {
                    Vector2 targetPosition = new Vector2(x, y);
                    var piece = _boardModel.GetBoardField(targetPosition);

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

        public bool CheckForCastling(ChessPiece chessPiece, ChessPiece targetPiece)
        {
            return chessPiece is KingPiece && targetPiece is RookPiece && chessPiece.ChessSide == targetPiece.ChessSide;
        }

        public Result<Action> MakeCastling(KingPiece king, RookPiece rook, ChessSide chessSide)
        {
            int direction = rook.Position.X > king.Position.X ? 1 : -1;
            int distance = GetAbsoluteDifferenceBetweenVectors(king.Position, rook.Position).X;

            if (!isPathBetweenEmpty(king.Position, rook.Position))
                return Result<Action>.Failure("Path between king and rook is not empty!");

            if (isKingCastlingPathUnderAttack(king.Position, direction, chessSide))
                return Result<Action>.Failure("King path is under attack!");

            Vector2 newKingPos = new Vector2(king.Position.X + 2 * direction, king.Position.Y);
            Vector2 newRookPos = new Vector2(newKingPos.X - direction, king.Position.Y);

            return Result<Action>.Success(() =>
            {
                _stepController.MakeChessPieceStep(king, newKingPos);
                _stepController.MakeChessPieceStep(rook, newRookPos);
            });
        }

        private bool isPathBetweenEmpty(Vector2 kingPos, Vector2 rookPos)
        {
            int startX = Math.Min(kingPos.X, rookPos.X) + 1;
            int endX = Math.Max(kingPos.X, rookPos.X);

            for (int PosX = startX; PosX < endX; PosX++)
            {
                Vector2 targetPosition = new Vector2(PosX, kingPos.Y);

                if (_boardModel.GetBoardField(targetPosition) != null)
                    return false;
            }

            return true;
        }

        private bool isKingCastlingPathUnderAttack(Vector2 kingPos, int direction, ChessSide side)
        {
            for (int i = 1; i <= 2; i++)
            {
                Vector2 checkPos = new Vector2(kingPos.X + i * direction, kingPos.Y);

                if (isFieldUnderAttack(checkPos, side))
                    return true;
            }

            return false;
        }

        private bool isFieldUnderAttack(Vector2 fieldPosition, ChessSide sideUnderAttack)
        {
            ChessSide enemySide = sideUnderAttack.GetOpposite();

            for (int y = 0; y < BoardModel._boardHeight; y++)
            {
                for (int x = 0; x < BoardModel._boardWidth; x++)
                {
                    Vector2 targetPosition = new Vector2(x, y);
                    ChessPiece piece = _boardModel.GetBoardField(targetPosition);

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

        public void ConfigureEnPassantTarget(ChessPiece piece, Vector2 startedPosition, Vector2 targetPosition)
        {
            _enPassantPieceTarget = null;

            if (piece is PawnPiece && Math.Abs(targetPosition.Y - startedPosition.Y) == 2)
                _enPassantPieceTarget = piece as PawnPiece;
        }

        public bool ValidateEnPassant(ChessPiece myPiece, Vector2 targetPosition)
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

        public Result<Action> ExecuteEnPassant(ChessPiece myPiece)
        {
            ChessSide chessSide = myPiece.ChessSide;

            Vector2 myPawnPosition = myPiece.Position;
            Vector2 enemyPawnPosition = _enPassantPieceTarget.Position;

            int myPawnPositionY = chessSide == ChessSide.Player ? myPawnPosition.Y - 1 : myPawnPosition.Y + 1;

            Vector2 targetPosition = new Vector2(enemyPawnPosition.X, myPawnPositionY);

            ChessPiece originalEnemy = _enPassantPieceTarget;

            _boardModel.ClearChessField(enemyPawnPosition);

            bool isKingUnderCheck = WouldKingBeUnderCheck(myPiece, targetPosition, chessSide);

            _boardModel.SetupChessPiece(enemyPawnPosition, originalEnemy);

            if (isKingUnderCheck == true)
                return Result<Action>.Failure("Your king would be in check!");

            return Result<Action>.Success(() =>
            {
                _boardModel.ClearChessField(enemyPawnPosition);
                _stepController.MakeChessPieceStep(myPiece, targetPosition);
            });
        }

        #endregion

        #region Pawn Promotion Rule

        public void CheckForPawnPromotion(ChessPiece piece)
        {
            if (piece is not PawnPiece)
                return;

            int promotionRow = piece.ChessSide == ChessSide.Player ? 0 : BoardModel._boardHeight - 1;

            if (piece.Position.Y == promotionRow)
            {
                DisplayView.WriteLine("\n--- PAWN PROMOTION! ---");
                DisplayView.WriteLine("Choose a piece: [Q]ueen, [R]ook, [B]ishop, [K]night");

                string input = DisplayView.ReadLine()?.ToUpper() ?? "Q";

                ChessPiece newPiece = input switch
                {
                    "R" => new RookPiece(piece.Position, piece.ChessSide),
                    "B" => new BishopPiece(piece.Position, piece.ChessSide),
                    "K" => new KnightPiece(piece.Position, piece.ChessSide),
                    _ => new QueenPiece(piece.Position, piece.ChessSide)
                };

                _boardModel.SetupChessPiece(piece.Position, newPiece);
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
            for (int y = 0; y < BoardModel._boardHeight; y++)
            {
                for (int x = 0; x < BoardModel._boardWidth; x++)
                {
                    Vector2 targetPosition = new Vector2(x, y);
                    ChessPiece currentPiece = _boardModel.GetBoardField(targetPosition);

                    if (currentPiece != null)
                        yield return currentPiece;
                }
            }
        }

        #endregion

        #region Check Pieces Movement

        private bool CanPieceMoveAnywhere(ChessPiece piece)
        {
            ChessSide side = piece.ChessSide;

            for (int y = 0; y < BoardModel._boardHeight; y++)
            {
                for (int x = 0; x < BoardModel._boardWidth; x++)
                {
                    Vector2 targetPosition = new Vector2(x, y);
                    ChessPiece occupant = _boardModel.GetBoardField(targetPosition);

                    if (occupant != null && occupant.ChessSide == side)
                        continue;

                    bool canPhysicallyMove = occupant == null ? piece.CheckMovement(targetPosition) : piece.CheckBeating(targetPosition);

                    ChessAction chessAction = occupant == null ? ChessAction.Movement : ChessAction.Beating;

                    if (canPhysicallyMove && isPathClear(piece, targetPosition, chessAction))
                    {
                        if (!WouldKingBeUnderCheck(piece, targetPosition, side))
                            return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region King Under Check Simulation

        public bool WouldKingBeUnderCheck(ChessPiece piece, Vector2 targetPosition, ChessSide chessSide)
        {
            Vector2 originalPosition = piece.Position;
            ChessPiece? targetPieceField = _boardModel.GetBoardField(targetPosition);

            _boardModel.SetupChessPiece(targetPosition, piece);
            _boardModel.ClearChessField(originalPosition);

            piece.SetPositionInternal(targetPosition);

            bool isKingUnderCheck = ValidateCheck(chessSide);

            _boardModel.SetupChessPiece(originalPosition, piece);
            _boardModel.SetupChessPiece(targetPosition, targetPieceField);

            piece.SetPositionInternal(originalPosition);

            return isKingUnderCheck;
        }

        #endregion

        #region Helper Methods

        private Vector2 GetAbsoluteDifferenceBetweenVectors(Vector2 firstPosition, Vector2 secondPosition)
        {
            Vector2 difference = firstPosition - secondPosition;
            return new Vector2(Math.Abs(difference.X), Math.Abs(difference.Y));
        }

        #endregion
    }
}
