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
        Gameboard.Instance.ChangeTilesLayers(availableMoves,"Highlight");
    }

    public override void Update()
    {
        base.Update();
        currentHover = Gameboard.Instance.GetCurrentHover();
        //IF
        if(Input.GetMouseButtonDown(0))
            stateMachine.ChangeState(TSM.selectionState);
    }

    public override void Exit()
    {
        base.Exit();
        Gameboard.Instance.ChangeTilesLayers(availableMoves,"Tile");
        availableMoves.Clear();
    }
}
