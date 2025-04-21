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
        // UIManager.Instance.Resume();
        TSM.DeselectMinion();
        DestroyAllMinions();
        
        // UIManager.Instance.RemoveWinnerScreen();
    }

    private void DestroyAllMinions(){
        for (int x = 0; x < Gameboard.TILE_COUNT_X; x++)
            for (int y = 0; y < Gameboard.TILE_COUNT_Y; y++)
                if(TSM.GetMinionUnit(new Vector2Int(x,y)) != null)
                    TSM.RemoveMinionFromBattleground(TSM.GetMinionUnit(new Vector2Int(x,y)), true);
    }

    public override void Update()
    {
        base.Update();
        stateMachine.ChangeState(TSM.selectionState);
    }

    public override void Exit()
    {
        base.Exit();
        TSM.ClearLogicVariables();
        SpawnPlayers();
        // UIManager.Instance.UpdateTurnText(currentPlayerTurn);
    }

    private void SpawnPlayers()
    {
        TSM.SpawnSingleMinion(TSM.GetTeamMinionSO(0,Team.Player1),Team.Player1,new Vector2Int(4,0));
        TSM.SpawnSingleMinion(TSM.GetTeamMinionSO(1,Team.Player1),Team.Player1,new Vector2Int(4,1));
        TSM.SpawnSingleMinion(TSM.GetTeamMinionSO(0,Team.Player2),Team.Player2,new Vector2Int(4,7));
        TSM.SpawnSingleMinion(TSM.GetTeamMinionSO(1,Team.Player2),Team.Player2,new Vector2Int(4,6));
    }
}
