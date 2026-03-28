using System;
using Cysharp.Threading.Tasks;

public class LevelEndUIState : BaseState<UIStateMachine.UIState>
{
    private bool _isWin;
    private readonly GameSettings _settings;
    private readonly UIManager _uiManager;

    public LevelEndUIState(UIStateMachine.UIState key, UIStateMachine.UIState nextStateKey, GameSettings settings, UIManager uiManager) : base(key)
    {
        NextStateKey = nextStateKey;
        _settings = settings;
        _uiManager = uiManager;
    }

    public override void OnEnter()
    {
        if (_isWin)
        {
            Complete();
        }
        else
        {
            DelayFail(_settings.levelFailedUIDelay);
        }
    }

    private async UniTask DelayFail(float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        _uiManager.ShowFail();
    }

    private void Complete()
    {
        _uiManager.ShowCompleted();
    }

    public override void OnExit()
    {
    }

    public void SetWin(bool isWin)
    {
        _isWin = isWin;
    }
}
