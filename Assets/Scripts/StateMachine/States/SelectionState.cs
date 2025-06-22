using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionState : BaseState
{
    private TurnStateMachine TSM;

    private Vector2Int currentHover;
    private List<Vector2Int> availableSelection;

    public SelectionState(TurnStateMachine stateMachine) : base("Selection", stateMachine)
    {
        TSM = stateMachine;
    }

    //  ENTER //
    public override void Enter()
    {
        base.Enter();
        TSM.DeselectMinion();
        TSM.DeselectAction();
        availableSelection = new List<Vector2Int>();
        UIManager.Instance.RemoveSelectedMinionUI();
        HighlightPlayerMinions();
    }

    private void HighlightPlayerMinions()
    {
        availableSelection = TSM.GetTeamAliveMinionPositions(TSM.GetCurrentPlayerTurn());
        Gameboard.Instance.ChangeTilesLayers(availableSelection,TileLayer.Highlight);
    }

    // UPDATE //
    public override void Update()
    {
        base.Update();
        currentHover = Gameboard.Instance.GetCurrentHover();
        if (CanMinionBeSelected())
        {
            MinionUnit selectedMinion = TSM.SelectMinion(currentHover);
            UIManager.Instance.SetupSelectedMinionUI(selectedMinion.minion);
            stateMachine.ChangeState(TSM.chooseState);
        }
    }

    private bool CanMinionBeSelected()
    {
        if(
            Input.GetMouseButtonDown(0) &&
            currentHover != -Vector2Int.one &&
            TSM.GetMinionUnit(currentHover) != null &&
            TSM.GetMinionUnit(currentHover)?.Team == TSM.GetCurrentPlayerTurn() &&
            TSM.GetMinionUnit(currentHover)?.minion.IsFainted() == false
            )
            return true;

        return false;
    }

    // EXIT //
    public override void Exit()
    {
        base.Exit();
        Gameboard.Instance.RestoreTilesLayers(availableSelection);
        availableSelection.Clear();   
    }

}
