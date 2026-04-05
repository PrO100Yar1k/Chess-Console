namespace Chess_Console.StateMachine.Base
{
    internal interface ISwitchableState
    {
        public void SwitchState<T>() where T : BaseState;
    }
}
