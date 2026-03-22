
namespace Chess_Console.Inputs
{
    internal class PlayerInput : IGeneralInput
    {
        private readonly InputHandler _inputHandler = new InputHandler();

        public Move GetInputMovement()
        {
            while (true) // to do
            {
                Console.WriteLine("\nEnter your movement (for example, e2 e3):");

                string input = Console.ReadLine();

                var result = _inputHandler.ParseMove(input);

                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.Error);
                    continue;
                }

                return result.Value;
            }
        }
    }
}
