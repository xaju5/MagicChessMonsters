using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : BaseState
{
    private TurnStateMachine TSM;
    public MoveState(TurnStateMachine stateMachine) : base("Move", stateMachine)
    {
        TSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        availableMoves = selectedMinion.minion.GetAvailableMoves(ref minionUnits, tileIndex, Gameboard.TILE_COUNT_X, Gameboard.TILE_COUNT_Y);
        Gameboard.Instance.ChangeTilesLayers(availableMoves,"Highlight");
    }

    public override void Update()
    {
        base.Update();
        //IF
        stateMachine.ChangeState(TSM.selectionState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
