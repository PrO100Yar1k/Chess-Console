using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_Console.StateMachine
{
    internal interface ISwitchableState
    {
        public void SwitchState<T>() where T : BaseState;
    }
}
