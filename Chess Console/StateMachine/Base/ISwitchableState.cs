namespace Chess_Console.StateMachine
{
    internal interface ISwitchableState
    {
        public void SwitchState<T>() where T : BaseState;
    }
}
