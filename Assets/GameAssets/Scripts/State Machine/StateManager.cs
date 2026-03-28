using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();

    private BaseState<EState> _currentState;

    private void Update()
    {
        if (_currentState is ITickableState tickableState)
        {
            tickableState.OnUpdate();
        }
    }

    private void FixedUpdate()
    {
        if (_currentState is IFixedTickableState fixedTickableState)
        {
            fixedTickableState.OnFixedUpdate();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }

    public void Initialize()
    {
        SetStates();
    }

    protected virtual void TransitionToState(EState stateKey)
    {
        if (_currentState != null && _currentState == States[stateKey])
        {
            return;
        }

        _currentState?.OnExit();
        _currentState = States[stateKey];
        _currentState.OnEnter();
    }

    protected abstract void SetStates();

    protected virtual void CleanUp()
    {
    }

    public void SetStateWithKey(EState stateKey)
    {
        TransitionToState(stateKey);
    }

    public EState GetCurrentState()
    {
        return _currentState.StateKey;
    }
}
