using Chess_Console.Core.Rules;
using Chess_Console.Core.Pieces.Base;
using Chess_Console.Core.Board;

namespace Chess_Console.Controllers
{
    internal class StepController
    {
        private readonly BoardModel _boardModel;
        private readonly RulesValidator _rulesValidator;

        public StepController(BoardModel boardModel, RulesValidator rulesValidator)
        {
            _boardModel = boardModel;
            _rulesValidator = rulesValidator;
        }

        public void MakeChessPieceStep(ChessPiece piece, Vector2 targetPosition)
        {
            Vector2 startedPosition = piece.Position;

            bool willCapturing = _boardModel.GetBoardField(targetPosition) != null;

            _rulesValidator.ConfigureEnPassantTarget(piece, startedPosition, targetPosition);
            _rulesValidator.UpdateFiftyMoveCounter(piece, willCapturing);

            _boardModel.SetupChessPiece(targetPosition, piece);
            _boardModel.ClearChessField(startedPosition);

            piece.SetPosition(targetPosition);

            _rulesValidator.CheckForPawnPromotion(piece);
            _rulesValidator.RecordPosition(piece.ChessSide);
        }
    }
}
