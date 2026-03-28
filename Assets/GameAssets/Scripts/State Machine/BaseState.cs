using System;

public abstract class BaseState<EState> where EState : Enum
{
    protected BaseState(EState key)
    {
        StateKey = key;
    }
    
    public EState StateKey { get; private set; }
    protected EState NextStateKey;
    public abstract void OnEnter();
    public abstract void OnExit();

}
