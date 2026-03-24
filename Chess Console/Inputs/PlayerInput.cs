namespace Chess_Console.Inputs
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
