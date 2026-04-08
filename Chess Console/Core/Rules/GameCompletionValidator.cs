using Chess_Console.Core.Common.Enums;
using Chess_Console.Infrastructure.Common;
using Chess_Console.Core.Common.Structures;

namespace Chess_Console.Core.Rules
{
    internal class GameCompletionValidator
    {
        private readonly RulesValidator _rulesValidator;

        public GameCompletionValidator(RulesValidator rulesValidator)
        {
            _rulesValidator = rulesValidator;
        }

        public Result<GameCompletionResult> ValidateGameCompletion(ChessSide chessSide)
        {
            if (_rulesValidator.ValidateCheckmate(chessSide))
                return Result<GameCompletionResult>.Success(new GameCompletionResult($"Checkmate for {chessSide}!", GameCompletionType.Win, chessSide.GetOpposite()));

            if (_rulesValidator.ValidateRepeatStepsDraw())
                return Result<GameCompletionResult>.Success(new GameCompletionResult($"Threefold repetition of the position for {chessSide.GetOpposite()}!", GameCompletionType.Draw));

            if (_rulesValidator.ValidateFiftyStepsNoBeatingDraw())
                return Result<GameCompletionResult>.Success(new GameCompletionResult($"Executed 50 Steps without beating!", GameCompletionType.Draw));

            if (_rulesValidator.ValidateInsufficientMaterial())
                return Result<GameCompletionResult>.Success(new GameCompletionResult($"There are no enough pieces to make checkmate!", GameCompletionType.Draw));

            if (_rulesValidator.ValidateStalemate(chessSide))
                return Result<GameCompletionResult>.Success(new GameCompletionResult($"No way! There are no moves for {chessSide}!", GameCompletionType.Stalemate));

            return Result<GameCompletionResult>.Failure("Game continues...");
        }
    }
}
