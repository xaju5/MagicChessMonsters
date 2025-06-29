using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameoverState : BaseState
{
    private TurnStateMachine TSM;

    public GameoverState(TurnStateMachine stateMachine) : base("GameOver", stateMachine)
    {
        TSM = stateMachine;
        
    }

    public override void Enter()
    {
        base.Enter();
        Team winner = TSM.GetWinner();
        UIManager.Instance.RemoveSelectedMinionUI();
        UIManager.Instance.EnableWinnerMenu(true, winner);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
        UIManager.Instance.EnableWinnerMenu(false);
    }

}
