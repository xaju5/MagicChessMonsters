using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnState : BaseState
{
    private TurnStateMachine TSM;
    private MinionUnit selectedMinion;
    private float restoreMagicAmount = 15f;
    public EndTurnState(TurnStateMachine stateMachine) : base("EndTurn", stateMachine)
    {
        TSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        selectedMinion = TSM.GetSelectedMinion();
        RefreshUnselectedMinionMagic();
    }

    private void RefreshUnselectedMinionMagic(){
        foreach (MinionUnit minionUnit in TSM.GetMinionUnitList(TSM.GetCurrentPlayerTurn()))
            if(minionUnit != selectedMinion) 
                minionUnit.RestoreMagic(restoreMagicAmount);
    }

    public override void Update()
    {
        base.Update();
        // if(isGameover) return;
        stateMachine.ChangeState(TSM.selectionState);
    }

    public override void Exit()
    {
        base.Exit();
        TSM.SwitchPlayerTurn();
        UIManager.Instance.UpdateTurnText(TSM.GetCurrentPlayerTurn());
    }
}
