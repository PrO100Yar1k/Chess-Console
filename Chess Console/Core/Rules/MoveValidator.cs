using Chess_Console.Core.Board;
using Chess_Console.Data.Enums;
using Chess_Console.Controllers;
using Chess_Console.Core.Pieces.Base;
using Chess_Console.Core.Pieces.Instances;
using Chess_Console.Infrastructure.Common;

namespace Chess_Console.Core.Rules
{
    internal class MoveValidator
    {
        private readonly RulesValidator _rulesValidator;
        private readonly StepController _stepController;
        private readonly BoardModel _boardModel;

        public MoveValidator(BoardModel boardModel, RulesValidator rulesValidator, StepController stepController)
        {
            _boardModel = boardModel;
            _rulesValidator = rulesValidator;
            _stepController = stepController;
        }

        public Result<Action> ValidateMovement(Vector2 startPosition, Vector2 targetPosition, ChessSide chessSide)
        {
            ChessPiece? chessPiece = _boardModel.GetBoardField(startPosition);
            ChessPiece? targetPiece = _boardModel.GetBoardField(targetPosition);

            if (chessPiece == null)
                return Result<Action>.Failure("Start field is empty!");

            if (chessPiece.ChessSide != chessSide)
                return Result<Action>.Failure("You cannot use enemy piece!");

            if (targetPiece == null)
            {
                if (chessPiece.CheckMovement(targetPosition))
                    return CheckMovementOverPiece(chessPiece, targetPosition, ChessAction.Movement, chessSide);

                if (_rulesValidator.ValidateEnPassant(chessPiece, targetPosition))
                    return _rulesValidator.ExecuteEnPassant(chessPiece);
            }

            else if (chessPiece.CheckBeating(targetPosition) && isEnemyChessSide(chessPiece, targetPiece))
                return CheckMovementOverPiece(chessPiece, targetPosition, ChessAction.Beating, chessSide);

            else if (_rulesValidator.CheckForCastling(chessPiece, targetPiece))
                return _rulesValidator.MakeCastling(chessPiece as KingPiece, targetPiece as RookPiece, chessSide);

            return Result<Action>.Failure("Invalid input, please try again!");
        }

        private Result<Action> CheckMovementOverPiece(ChessPiece piece, Vector2 targetPosition, ChessAction chessAction, ChessSide chessSide)
        {
            if (!_rulesValidator.isPathClear(piece, targetPosition, chessAction))
                return Result<Action>.Failure("You cannot jump over other pieces!");

            if (_rulesValidator.WouldKingBeUnderCheck(piece, targetPosition, chessSide))
                return Result<Action>.Failure("Invalid move: Your king (is / will be) in check!");

            return Result<Action>.Success(() => _stepController.MakeChessPieceStep(piece, targetPosition));
        }

        private bool isEnemyChessSide(ChessPiece chessPiece, ChessPiece targetChessPiece)
        {
            return targetChessPiece != null && chessPiece.ChessSide != targetChessPiece.ChessSide;
        }
    }
}
