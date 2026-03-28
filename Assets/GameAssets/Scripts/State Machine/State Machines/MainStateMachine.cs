public class MainStateMachine : StateManager<MainStateMachine.MainState>
{
    public enum MainState
    {
        Start,
        Game,
        Finish,
    }

    public void Inject()
    {
        // No dependencies needed
    }

    private void OnEnable()
    {
        GameEvents.OnLevelEnd += OnLevelEnd;
        GameEvents.OnLevelStart += OnLevelStart;
    }

    private void OnDisable()
    {
        GameEvents.OnLevelEnd -= OnLevelEnd;
        GameEvents.OnLevelStart -= OnLevelStart;
    }

    protected override void SetStates()
    {
        var start = new StartGameState(MainState.Start, MainState.Game);
        var game = new PlayGameState(MainState.Game, MainState.Finish);
        var finish = new FinishGameState(MainState.Finish, MainState.Start);

        States.Add(MainState.Start, start);
        States.Add(MainState.Game, game);
        States.Add(MainState.Finish, finish);

        SetStateWithKey(MainState.Start);
    }

    private void OnLevelEnd(bool isWin, int levelIndex)
    {
        if (GetCurrentState() == MainState.Finish)
        {
            return;
        }
        SetStateWithKey(MainState.Finish);
    }

    private void OnLevelStart(int levelIndex, Level level)
    {
        if (GetCurrentState() == MainState.Game)
        {
            return;
        }
        SetStateWithKey(MainState.Game);
    }
}
