public class PlayGameState : BaseState<MainStateMachine.MainState>
{
    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    public PlayGameState(MainStateMachine.MainState key, MainStateMachine.MainState nextStateKey) : base(key)
    {
        NextStateKey = nextStateKey;
    }
}
