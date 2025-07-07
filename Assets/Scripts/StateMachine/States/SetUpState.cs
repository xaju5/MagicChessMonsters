using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpState : BaseState
{
    private TurnStateMachine TSM;
    public SetUpState(TurnStateMachine stateMachine) : base("SetUp", stateMachine)
    {
        TSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        TSM.DeselectMinion();
        TSM.DeselectAction();
        TSM.DeselectTargetPosition();
        TSM.SetAnimationTimer(AnimationTimer.None);
        DestroyAllMinions();
        TSM.ClearLogicVariables();
        TSM.SpawnAllMinions();
        SummonPlayers();
        UIManager.Instance.UpdateTurnText(TSM.GetCurrentPlayerTurn());
    }

    private void DestroyAllMinions()
    {
        for (int x = 0; x < Gameboard.TILE_COUNT_X; x++)
            for (int y = 0; y < Gameboard.TILE_COUNT_Y; y++)
                if (TSM.GetMinionUnit(new Vector2Int(x, y)) != null)
                    TSM.RemoveMinionFromBattleground(TSM.GetMinionUnit(new Vector2Int(x, y)), true);
    }    
    private void SummonPlayers()
    {
        List<MinionUnit> trainers = TSM.GetTrainers();
        TSM.SummonMinion(trainers[0], new Vector2Int(4, 0));
        TSM.SummonMinion(trainers[1], new Vector2Int(4, 7));
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
