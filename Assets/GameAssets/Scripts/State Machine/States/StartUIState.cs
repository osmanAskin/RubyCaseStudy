namespace RubyCase.StateMachine
{
    public class StartUIState : BaseState<UIStateMachine.UIState>
    {
        public StartUIState(UIStateMachine.UIState key, UIStateMachine.UIState nextStateKey) : base(key)
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
