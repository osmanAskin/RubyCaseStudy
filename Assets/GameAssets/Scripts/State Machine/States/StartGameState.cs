public class StartGameState : BaseState<MainStateMachine.MainState>
{
    public StartGameState(MainStateMachine.MainState key, MainStateMachine.MainState nextState) : base(key)
    {
        NextStateKey = nextState;
    }

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }
}
