using Chess_Console.Inputs;
using Chess_Console.Views;

namespace Chess_Console.StateMachine.GameState
{
    internal class GameplayState : BaseState
    {
        private CancellationTokenSource _cancellationTokenSource = default;

        private IGeneralInput _playerInput = default;
        private IGeneralInput _enemyInput = default;

        private GameBoard _board = default;

        public GameplayState(ISwitchableState switchable) : base(switchable)
        {
            _board = new GameBoard();

            _playerInput = new PlayerInput();
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

                Move playerMovement = _playerInput.GetInputMovement();

                //Console.WriteLine($"{playerMovement.StartPoint.X} {playerMovement.StartPoint.Y}");
                //Console.WriteLine($"{playerMovement.FinalPoint.X}:{playerMovement.FinalPoint.Y}");

                if (!_board.ValidateMovement(playerMovement.StartPoint, playerMovement.FinalPoint))
                    Console.WriteLine("Incorrect input!!!");

                //make movement

                //Move enemyMovement = _enemyInput.GetInputMovement();
                //make movement
            }

            return Task.CompletedTask;
        }
    }
}
