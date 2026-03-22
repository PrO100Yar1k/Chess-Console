namespace Chess_Console.Others
{
    internal static class GameConstants
    {
        public const char KingChar = 'K';
        public const char QueenChar = 'Q';
        public const char BishopChar = 'B';
        public const char KnightChar = 'V';
        public const char RookChar = 'R';

        public const char PawnChar = 'P';
        public const char EmptyChar = '.';

        public static readonly string Backline = new string(new char[] {
            RookChar, KnightChar, BishopChar, QueenChar, KingChar, BishopChar, KnightChar, RookChar
        });
    }
}
