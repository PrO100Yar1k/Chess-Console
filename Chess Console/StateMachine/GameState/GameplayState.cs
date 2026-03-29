using Chess_Console.Data.Enums;
using Chess_Console.Inputs;
using Chess_Console.Others;
using Chess_Console.Views;
using System.ComponentModel;

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

            _playerInputHandler = new InputHandler();
            _playerInput = new PlayerInput(_playerInputHandler);

            _enemyInputHandler = new InputHandler(); //
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

        private async Task GameplayLoop(CancellationToken token) // to do
        {
            _board.DisplayBoard();

            while (!token.IsCancellationRequested)
            {
                if (ValidateCheckMate(ChessSide.Player))
                    break;

                if (_board.ValidateChessStalemate(ChessSide.Player))
                    break;

                ExecuteTurn(ChessSide.Player, _playerInput);

                // ----------------------------------------------- \\

                if (ValidateCheckMate(ChessSide.Enemy))
                    break;

                if (_board.ValidateChessStalemate(ChessSide.Enemy))
                    break;

                ExecuteTurn(ChessSide.Enemy, _enemyInput);
            }

            await Task.Delay(50);
        }

        private bool ValidateGameCompletion()
        {
            return false;
        }

        private void ExecuteTurn(ChessSide chessSide, IGeneralInput inputSource)
        {
            while (true)
            {
                Move movement = inputSource.GetInputMovement();

                var validateResult = _board.ValidateMovement(movement.StartPoint, movement.FinalPoint, chessSide);

                if (validateResult.IsSuccess)
                {
                    validateResult.Value.Invoke();
                    break;
                }

                else Console.WriteLine(validateResult.Error);
            }

            ChessSide opponentSide = chessSide.GetOpposite();

            if (_board.ValidateCheck(opponentSide))
                Console.WriteLine(GetCheckMessage(opponentSide));

            _board.DisplayBoard();
        }

        private bool ValidateCheckMate(ChessSide side)
        {
            if (!_board.ValidateCheckmate(side))
                return false;

            _switchable.SwitchState<GameCompletionState>();
            GameEvents.Instance.GameCompletion(side);

            return true;
        }

        private string GetCheckMessage(ChessSide opponent)
        {
            return opponent == ChessSide.Enemy ? "Nice! Enemy have check!" : "Warning! You have check!";
        }

        private void DisplayMovement(Move movement)
        {
            Console.WriteLine($"Start Point: {movement.StartPoint}");
            Console.WriteLine($"Final Point: {movement.FinalPoint}");

            //Console.WriteLine($"From: {movement.StartPoint.X} {movement.StartPoint.Y}");
            //Console.WriteLine($"To: {movement.FinalPoint.X} {movement.FinalPoint.Y}");
        }
    }
}
