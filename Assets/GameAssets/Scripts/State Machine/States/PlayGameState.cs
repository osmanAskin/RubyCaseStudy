namespace RubyCase.StateMachine
{
    public class PlayGameState : BaseState<MainStateMachine.MainState>
    {
        public PlayGameState(MainStateMachine.MainState key, MainStateMachine.MainState nextStateKey) : base(key)
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
}
