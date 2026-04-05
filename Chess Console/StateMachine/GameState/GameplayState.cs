using Chess_Console.Views;
using Chess_Console.Core.Board;
using Chess_Console.Core.Rules;
using Chess_Console.Core.Common.Enums;
using Chess_Console.StateMachine.Base;
using Chess_Console.Infrastructure.Common;
using Chess_Console.Core.Common.Structures;
using Chess_Console.Infrastructure.Inputs.Handlers;
using Chess_Console.Infrastructure.Inputs.Instances;
using Chess_Console.Infrastructure.Inputs.Interfaces;

namespace Chess_Console.StateMachine.GameState
{
    internal class GameplayState : BaseState
    {
        private CancellationTokenSource _cancellationTokenSource;

        private readonly RulesValidator _rulesValidator;
        private readonly StepController _stepController;
        private readonly BoardRenderer _boardRenderer;
        private readonly MoveValidator _moveValidator;
        private readonly BoardModel _boardModel;

        private readonly IInputHandler _playerInputHandler;
        private readonly IInputHandler _enemyInputHandler;

        private readonly IGeneralInput _playerInput;
        private readonly IGeneralInput _enemyInput;

        public GameplayState(ISwitchableState switchable) : base(switchable)
        {
            _boardModel = new BoardModel();

            _boardRenderer = new BoardRenderer(_boardModel);
            _rulesValidator = new RulesValidator(_boardModel);

            _stepController = new StepController(_boardModel, _rulesValidator);
            _moveValidator = new MoveValidator(_boardModel, _rulesValidator, _stepController);

            _rulesValidator.SetStepController(_stepController);

            _playerInputHandler = new PlayerInputHandler();
            _playerInput = new PlayerInput(_playerInputHandler);

            _enemyInputHandler = new PlayerInputHandler(); //
            _enemyInput = new PlayerInput(_enemyInputHandler); //
        }

        public override void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _ = GameplayLoop(_cancellationTokenSource.Token);
        }

        public override void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private async Task GameplayLoop(CancellationToken token)
        {
            _boardRenderer.DisplayBoard();

            while (!token.IsCancellationRequested)
            {
                if (!ExecuteTurn(ChessSide.Player, _playerInput, token))
                    break;

                if (token.IsCancellationRequested)
                    break;

                if (!ExecuteTurn(ChessSide.Enemy, _enemyInput, token))
                    break;
            }

            await Task.Delay(50);
        }

        private bool ExecuteTurn(ChessSide chessSide, IGeneralInput inputSource, CancellationToken token)
        {
            ChessSide opponentSide = chessSide.GetOpposite();

            var gameCompletionResult = ValidateGameCompletion(chessSide);

            if (gameCompletionResult.IsSuccess) { 
                ExecuteGameCompletion(gameCompletionResult.Value);
                return false;
            }

            if (token.IsCancellationRequested)
                return false;

            var movement = ValidateMovement(chessSide, inputSource);
            ExecuteMovement(chessSide, movement.Value);

            return true;
        }

        #region Movement Validation & Execution

        private Result<Action> ValidateMovement(ChessSide chessSide, IGeneralInput inputSource)
        {
            while (true)
            {
                Move movement = inputSource.GetInputMovement();

                var validateResult = _moveValidator.ValidateMovement(movement.StartPoint, movement.FinalPoint, chessSide);

                if (validateResult.IsSuccess)
                    return validateResult;

                DisplayView.WriteLine(validateResult.Error);
            }
        }

        private void ExecuteMovement(ChessSide chessSide, Action movementAction)
        {
            movementAction.Invoke();

            ChessSide opponentSide = chessSide.GetOpposite();

            if (_rulesValidator.ValidateCheck(opponentSide))
                DisplayView.WriteLine(GetCheckMessage(opponentSide));

            _boardRenderer.DisplayBoard();
        }

        #endregion

        #region Game Completion

        private Result<GameCompletionResult> ValidateGameCompletion(ChessSide chessSide)
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

        private void ExecuteGameCompletion(GameCompletionResult result)
        {
            _switchable.SwitchState<GameCompletionState>();
            GameEvents.Instance.GameCompletion(result);
        }

        #endregion

        #region Display Coordinates of Executed Movement

        private void DisplayCoordinatesOfMovement(Move movement)
        {
            DisplayView.WriteLine($"Start Point: {movement.StartPoint}");
            DisplayView.WriteLine($"Final Point: {movement.FinalPoint}");
        }

        #endregion

        #region Helper Methods

        private string GetCheckMessage(ChessSide opponent)
        {
            return opponent == ChessSide.Enemy ? "Nice! Enemy have check!" : "Warning! You have check!";
        }

        #endregion
    }
}
