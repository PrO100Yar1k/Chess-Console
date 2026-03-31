using Chess_Console.Views;
using Chess_Console.Others;
using Chess_Console.Inputs;
using Chess_Console.Data.Enums;
using Chess_Console.Data.Structures;

namespace Chess_Console.StateMachine.GameState
{
    internal class GameplayState : BaseState
    {
        private CancellationTokenSource _cancellationTokenSource;

        private readonly IInputHandler _playerInputHandler;
        private readonly IInputHandler _enemyInputHandler;

        private readonly IGeneralInput _playerInput;
        private readonly IGeneralInput _enemyInput;

        private readonly GameBoard _board;

        public GameplayState(ISwitchableState switchable) : base(switchable)
        {
            _board = new GameBoard();

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
            _board.DisplayBoard();

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

                var validateResult = _board.ValidateMovement(movement.StartPoint, movement.FinalPoint, chessSide);

                if (validateResult.IsSuccess)
                    return validateResult;

                Console.WriteLine(validateResult.Error);
            }
        }

        private void ExecuteMovement(ChessSide chessSide, Action movementAction)
        {
            movementAction.Invoke();

            ChessSide opponentSide = chessSide.GetOpposite();

            if (_board.ValidateCheck(opponentSide))
                Console.WriteLine(GetCheckMessage(opponentSide));

            _board.DisplayBoard();
        }

        #endregion

        #region Game Completion

        private Result<GameCompletionResult> ValidateGameCompletion(ChessSide chessSide)
        {
            if (_board.ValidateCheckmate(chessSide))
                return Result<GameCompletionResult>.Success(new GameCompletionResult($"Checkmate for {chessSide}!", GameCompletionType.Win, chessSide.GetOpposite()));

            if (_board.ValidateRepeatStepsDraw())
                return Result<GameCompletionResult>.Success(new GameCompletionResult($"Threefold repetition of the position for {chessSide.GetOpposite()}!", GameCompletionType.Draw));

            if (_board.ValidateFiftyStepsNoBeatingDraw())
                return Result<GameCompletionResult>.Success(new GameCompletionResult($"Executed 50 Steps without beating!", GameCompletionType.Draw));

            if (_board.ValidateInsufficientMaterial())
                return Result<GameCompletionResult>.Success(new GameCompletionResult($"There are no enough pieces to make checkmate!", GameCompletionType.Draw));

            if (_board.ValidateChessStalemate(chessSide))
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
            Console.WriteLine($"Start Point: {movement.StartPoint}");
            Console.WriteLine($"Final Point: {movement.FinalPoint}");
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
