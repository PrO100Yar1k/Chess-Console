using Chess_Console.StateMachine.Controller;
using Chess_Console.StateMachine.GameState;

namespace Chess_Console
{
    internal class EntryPoint
    {
        private static void Main()
        {
            GameStateController gameStateController = new GameStateController();
            gameStateController.SwitchState<GameplayState>();
        }
    }
}