using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionState : BaseState
{
    private TurnStateMachine TSM;

    private Vector2Int currentHover;
    private List<Vector2Int> availableMoves;

    public SelectionState(TurnStateMachine stateMachine) : base("Selection", stateMachine)
    {
        TSM = stateMachine;
    }

    //  ENTER //
    public override void Enter()
    {
        base.Enter();
        TSM.DeselectMinion();
        availableMoves = new List<Vector2Int>();
        //     UIManager.Instance.RemoveSelectedMinionUI();
        HighlightPlayerMinions();
    }

    private void HighlightPlayerMinions()
    {
        availableMoves = TSM.GetTeamMinionPositions(TSM.GetCurrentPlayerTurn());
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
            TSM.GetMinionUnits(currentHover.x, currentHover.y) != null &&
            TSM.GetMinionUnits(currentHover.x, currentHover.y)?.Team == TSM.GetCurrentPlayerTurn()
            )
            return true;

        return false;
    }

    // EXIT //
    public override void Exit()
    {
        base.Exit();
        TSM.SelectMinion(currentHover);
        Gameboard.Instance.ChangeTilesLayers(availableMoves,"Tile");
        availableMoves.Clear();
    //     UIManager.Instance.SetupSelectedMinionUI(selectedMinion.minion);
    }

}
