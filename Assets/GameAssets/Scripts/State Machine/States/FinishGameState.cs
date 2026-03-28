public class FinishGameState : BaseState<MainStateMachine.MainState>
{
    public FinishGameState(MainStateMachine.MainState key, MainStateMachine.MainState nextStateKey) : base(key)
    {
        NextStateKey = nextStateKey;
    }

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }
}
