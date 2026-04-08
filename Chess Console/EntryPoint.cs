using Chess_Console.StateMachine.Base;
using Chess_Console.StateMachine.GameState;

using Microsoft.Extensions.DependencyInjection;

namespace Chess_Console
{
    internal class EntryPoint
    {
        private static void Main()
        {
            IServiceProvider serviceProvider = DependencyInjectionConfiguration.ConfigureServices();
            ISwitchableState gameStateController = serviceProvider.GetRequiredService<ISwitchableState>();

            gameStateController.SwitchState<GameplayState>();
        }
    }
}