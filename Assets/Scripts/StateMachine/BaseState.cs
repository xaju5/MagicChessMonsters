using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseState
{
    public string name;
    protected StateMachine stateMachine;
    private bool DEBUG_MESSAGES = true;

    public BaseState(string name, StateMachine stateMachine)
    {
        this.name = name;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() {
        if (DEBUG_MESSAGES) Debug.Log($"Entering {name}");
    }
    public virtual void Update() {}
    public virtual void Exit() {
        if (DEBUG_MESSAGES) Debug.Log($"Exiting {name}");
    }
}
