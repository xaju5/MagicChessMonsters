using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : BaseState
{
    private TurnStateMachine TSM;
    private Vector2Int currentHover;
    private List<Vector2Int> availableMoves;
    private MinionUnit selectedMinion;
    public MoveState(TurnStateMachine stateMachine) : base("Move", stateMachine)
    {
        TSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        selectedMinion = TSM.GetSelectedMinion();
        availableMoves = selectedMinion.minion.GetAvailableMoves(ref TSM.GetMinionUnitsArray(), selectedMinion.MinionIndex, Gameboard.TILE_COUNT_X, Gameboard.TILE_COUNT_Y);
        Gameboard.Instance.ChangeTilesLayers(availableMoves,TileLayer.Highlight);
    }

    public override void Update()
    {
        base.Update();
        currentHover = Gameboard.Instance.GetCurrentHover();
        if(Input.GetMouseButtonDown(0)){
            if(currentHover == -Vector2Int.one){
                stateMachine.ChangeState(TSM.selectionState);
                return;
            }
            if(TSM.GetMinionUnit(currentHover) == null){
                if(IsValidMove(currentHover)){
                    MoveSelectedMinion(currentHover);
                    stateMachine.ChangeState(TSM.endTurnState);
                }
            }
            // else if(minionUnits[currentHover.x, currentHover.y].Team == currentPlayerTurn){
            //     SwitchSelectMinion(currentHover);
            // }
        }
            
    }

    private bool IsValidMove(Vector2Int index)
    {
        foreach (Vector2Int availableIndex in availableMoves)
            if (availableIndex == index)
                return true;
        return false;
    }

    private void MoveSelectedMinion(Vector2Int newPosition){
        if(selectedMinion.MinionIndex == newPosition)
            throw new System.Exception("ERROR: New Position and current position are the same.");
        
        TSM.UpdateMinionPositionInArray(selectedMinion, newPosition);
        selectedMinion.MoveMinionUnit(newPosition);
    }

    public override void Exit()
    {
        base.Exit();
        Gameboard.Instance.RestoreTilesLayers(availableMoves);
        availableMoves.Clear();
    }
}
