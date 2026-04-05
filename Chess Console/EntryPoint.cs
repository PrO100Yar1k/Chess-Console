using Chess_Console.StateMachine.Base;
using Chess_Console.StateMachine.Manager;
using Chess_Console.StateMachine.GameState;

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