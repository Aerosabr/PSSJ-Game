using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();

    protected BaseState<EState> CurrentState;

    protected bool IsTransitioningState = false;

    void Start() 
    {
        CurrentState.EnterState();
    }
    void Update() 
    {
        EState nextStateKey = CurrentState.GetState();

        if(!IsTransitioningState && nextStateKey.Equals(CurrentState.StateKey))
        {
            CurrentState.UpdateState();
        }
        else
        {
            TransitionToState(nextStateKey);
        }
    }
    public void TransitionToState(EState statekey)
    {
        IsTransitioningState = true;
        CurrentState.ExitState();
        CurrentState = States[statekey];
        CurrentState.EnterState();
        IsTransitioningState = false;
    }
    void OnTriggerEnter(Collider other) 
    {
        CurrentState.OnTriggerEnter(other);
    }
    void OnTriggerStay(Collider other)
	{
		CurrentState.OnTriggerStay(other);
	}
	void OnTriggerExit(Collider other)
	{
		CurrentState.OnTriggerExit(other);
	}

}
