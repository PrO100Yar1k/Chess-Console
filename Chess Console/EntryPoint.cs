using Chess_Console.Inputs;
using Chess_Console.StateMachine.Controller;
using Chess_Console.StateMachine.GameState;
using Chess_Console.Views;

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