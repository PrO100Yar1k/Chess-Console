using Chess_Console.Infrastructure.Inputs.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Chess_Console.Infrastructure.Inputs.Instances
{
    internal class EnemyInput : IGeneralInput
    {
        private readonly IInputHandler _inputHandler;

        public EnemyInput([FromKeyedServices("Enemy Input")] IInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
        }

        public Move GetInputMovement() // to do
        {
            throw new NotImplementedException();
        }
    }
}
