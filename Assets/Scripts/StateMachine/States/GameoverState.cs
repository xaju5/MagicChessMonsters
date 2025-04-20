using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameoverState : BaseState
{
    private TurnStateMachine TSM;
    private Vector2Int currentHover;
    private List<Vector2Int> availableMoves;
    private MinionUnit selectedMinion;
    public GameoverState(TurnStateMachine stateMachine) : base("Attack", stateMachine)
    {
        TSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Update()
    {
        base.Update();
        stateMachine.ChangeState(TSM.selectionState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
