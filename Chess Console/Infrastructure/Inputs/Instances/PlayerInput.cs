using Chess_Console.Infrastructure.Inputs.Interfaces;
using Chess_Console.Views;

namespace Chess_Console.Infrastructure.Inputs.Instances
{
    internal class PlayerInput : IGeneralInput
    {
        private readonly IInputHandler _inputHandler;

        public PlayerInput(IInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
        }

        public Move GetInputMovement()
        {
            while (true)
            {
                DisplayView.WriteLine("\nEnter your movement (for example, e2 e3):");

                var input = DisplayView.ReadLine();
                var result = _inputHandler.ParseMove(input);

                if (!result.IsSuccess)
                {
                    DisplayView.WriteLine(result.Error);
                    continue;
                }

                return result.Value;
            }
        }
    }
}
