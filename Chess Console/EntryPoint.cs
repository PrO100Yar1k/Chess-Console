using Chess_Console.StateMachine.Base;
using Chess_Console.StateMachine.GameState;
using Chess_Console.StateMachine.Manager;

namespace Chess_Console
{
    internal class EntryPoint
    {
        private static void Main()
        {
            ISwitchableState gameStateController = new GameStateController();
            gameStateController.SwitchState<GameplayState>();
        }
    }
}