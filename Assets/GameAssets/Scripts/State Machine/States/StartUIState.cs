public class StartUIState : BaseState<UIStateMachine.UIState>
{
    private UIManager _uiManager;

    public StartUIState(UIStateMachine.UIState key, UIStateMachine.UIState nextStateKey, UIManager uiManager) : base(key)
    {
        NextStateKey = nextStateKey;
        _uiManager = uiManager;
    }

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }
}
