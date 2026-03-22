using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_Console.Inputs
{
    internal interface IInputHandler
    {
        public Result<Move> ParseMove(string input);
        public Result<Vector2> ParsePosition(string input);
    }
}
