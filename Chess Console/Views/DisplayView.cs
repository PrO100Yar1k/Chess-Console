namespace Chess_Console.Views
{
    internal static class DisplayView
    {
        public static void Write(string? message = null)
        {
            Console.Write(message);
        }

        public static void WriteLine(string? message = null)
        {
            Console.WriteLine(message);
        }

        public static string? ReadLine()
        {
            return Console.ReadLine();
        }
    }
}
