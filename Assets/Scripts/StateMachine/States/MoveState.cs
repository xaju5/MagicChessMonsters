using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : BaseState
{
    private TurnStateMachine TSM;
    private Vector2Int targetPosition;
    private MinionUnit selectedMinion;
    public MoveState(TurnStateMachine stateMachine) : base("Move", stateMachine)
    {
        TSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        selectedMinion = TSM.GetSelectedMinion();
        targetPosition = TSM.GetTargetPosition();
    }

    public override void Update()
    {
        base.Update();
        MoveSelectedMinion();
        stateMachine.ChangeState(TSM.endTurnState);    
    }

    private void MoveSelectedMinion(){
        if(selectedMinion.MinionIndex == targetPosition)
            throw new System.Exception("ERROR: New Position and current position are the same.");
        
        TSM.UpdateMinionPositionInArray(selectedMinion, targetPosition);
        selectedMinion.MoveMinionUnit(targetPosition);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
