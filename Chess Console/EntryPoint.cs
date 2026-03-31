using Chess_Console.StateMachine;
using Chess_Console.StateMachine.GameState;
using Chess_Console.StateMachine.Controller;

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