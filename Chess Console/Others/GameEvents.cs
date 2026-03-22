using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_Console.Others
{
    internal sealed class GameEvents
    {
        private static readonly Lazy<GameEvents> _lazy =
                new Lazy<GameEvents>(() => new GameEvents());

        public static GameEvents Instance => _lazy.Value;

        private GameEvents()
        {

        }
    }
}
