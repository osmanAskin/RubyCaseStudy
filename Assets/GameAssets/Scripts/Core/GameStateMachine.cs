public class GameStateMachine
{
    private IGameState _currentState;

    public void ChangeState(IGameState newState)
    {
        _currentState?.OnExit();
        _currentState = newState;
        _currentState.OnEnter();
    }

    public IGameState GetCurrentState()
    {
        return _currentState;
    }
}