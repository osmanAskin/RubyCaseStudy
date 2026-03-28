public class InGameUIState : BaseState<UIStateMachine.UIState>
{
    private readonly UIManager _uiManager;

    public InGameUIState(UIStateMachine.UIState key, UIStateMachine.UIState nextStateKey, UIManager uiManager) : base(key)
    {
        NextStateKey = nextStateKey;
        _uiManager = uiManager;
    }

    public override void OnEnter()
    {
        _uiManager.ShowGame();
    }

    public override void OnExit()
    {
    }
}
