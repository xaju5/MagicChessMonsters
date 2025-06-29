using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    BaseState currentState;
    protected bool isGamePaused;

    void Start()
    {
        isGamePaused = false;
        UIManager.Instance.resumeEvent.AddListener(TogglePause);
        currentState = GetInitialState();
        if (currentState != null)
            currentState.Enter();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !currentState.name.Equals("GameOver"))
            TogglePause();
        if (currentState != null && !isGamePaused)
            currentState.Update();
    }

    public void ChangeState(BaseState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
    protected virtual BaseState GetInitialState()
    {
        return null;
    }

    private void OUI()
    {
        string content = currentState != null ? currentState.name : "(no current state)";
        GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
    }

    //Pause
    protected void TogglePause()
    {
        SetIsGamePaused(!isGamePaused);
    }
    private void SetIsGamePaused(bool isGamePaused)
    {
        this.isGamePaused = isGamePaused;
        UIManager.Instance.EnablePauseMenu(isGamePaused);
    }

}
