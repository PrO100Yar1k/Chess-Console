using Chess_Console.Data.Enums;
using Chess_Console.Inputs;
using Chess_Console.Others;
using Chess_Console.Views;

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

            //_enemyInputHandler = new InputHandler();
            _enemyInput = new EnemyInput();
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

        private Task GameplayLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _board.DisplayBoard();

                // ----------------------------------------------- \\

                if (_board.ValidateCheckmate(ChessSide.Player))
                {
                    _switchable.SwitchState<GameCompletionState>();
                    GameEvents.Instance.GameCompletion(ChessSide.Player);

                    return Task.CompletedTask;
                }

                Move playerMovement = _playerInput.GetInputMovement();
                //DisplayMovement(playerMovement);

                if (!_board.ValidateMovement(playerMovement.StartPoint, playerMovement.FinalPoint, ChessSide.Player))
                {
                    Console.WriteLine("Incorrect input!!!");
                    continue;
                }

                if (_board.ValidateCheck(ChessSide.Enemy))
                    Console.WriteLine("Nice! Enemy have check!");



                // ----------------------------------------------- \\



                if (_board.ValidateCheckmate(ChessSide.Enemy))
                {
                    _switchable.SwitchState<GameCompletionState>();
                    GameEvents.Instance.GameCompletion(ChessSide.Enemy);

                    return Task.CompletedTask;
                }

                //Move enemyMovement = _enemyInput.GetInputMovement();
                //DisplayMovement(enemyMovement);

                if (_board.ValidateCheck(ChessSide.Player))
                    Console.WriteLine("Warning! You have check!");
            }

            return Task.CompletedTask;
        }

        private void ExecureTurn()
        {
            // to do
        }

        private void DisplayMovement(Move movement)
        {
            Console.WriteLine($"From: {movement.StartPoint.X} {movement.StartPoint.Y}");
            Console.WriteLine($"To: {movement.FinalPoint.X} {movement.FinalPoint.Y}");
        }
    }
}
