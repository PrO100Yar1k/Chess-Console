using Chess_Console.Core.Common.Interfaces;

namespace Chess_Console.StateMachine.Base
{
    internal interface ISwitchableState : IServiceInitializable
    {
        public void SwitchState<State>() where State : BaseState;

        public bool CheckStateForActivity<State>() where State : BaseState;
    }
}
