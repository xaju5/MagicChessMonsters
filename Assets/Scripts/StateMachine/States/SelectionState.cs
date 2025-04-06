using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionState : BaseState
{
    private TurnStateMachine TSM;

    private Vector2Int currentHover;

    public SelectionState(TurnStateMachine stateMachine) : base("Selection", stateMachine)
    {
        TSM = stateMachine;
    }

    //  ENTER //
    public override void Enter()
    {
        base.Enter();
        TSM.DeselectMinion();
        HighlightPlayerMinions();
    }

    private void HighlightPlayerMinions()
    {
        availableMoves = GetTeamMinionPositions(currentPlayerTurn);
        Gameboard.Instance.ChangeTilesLayers(availableMoves,"Highlight");
    }

    // UPDATE //
    public override void Update()
    {
        base.Update();
        currentHover = Gameboard.Instance.GetCurrentHover();
        if(CanMinionBeSelected())
            stateMachine.ChangeState(TSM.moveState);
    }

    private bool CanMinionBeSelected()
    {
        if(
            Input.GetMouseButtonDown(0) &&
            currentHover != -Vector2Int.one &&
            minionUnits[currentHover.x, currentHover.y] != null &&
            minionUnits[currentHover.x, currentHover.y]?.Team == currentPlayerTurn
            )
            return true;

        return false;
    }

    // EXIT //
    public override void Exit()
    {
        base.Exit();
    }
}
