using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    private TurnStateMachine TSM;
    private MinionUnit selectedMinion;
    private MinionUnit targetMinion;
    private Action selectedAction;
    private Vector2Int targetPosition;
    private List<ActionUnit> pendingAnimations = new List<ActionUnit>();
    public AttackState(TurnStateMachine stateMachine) : base("Attack", stateMachine)
    {
        TSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        selectedMinion = TSM.GetSelectedMinion();
        selectedAction = TSM.GetSelectedAction();
        targetPosition = TSM.GetTargetPosition();
        TSM.SetAnimationTimer(AnimationTimer.None);
    }

    public override void Update()
    {
        base.Update();
        switch (TSM.GetAnimationTimer())
        {   
            case AnimationTimer.None:      
                MakeSelectedAttack();  
                return;

            case AnimationTimer.Waiting:
                return;

            case AnimationTimer.Finished:
                if(TSM.GetWinner() != Team.None){
                    stateMachine.ChangeState(TSM.gameoverState);
                    return;
                }
                stateMachine.ChangeState(TSM.endTurnState);
                return;
            
            default:
                throw new System.Exception("Error: Wrong Animation Timer.");
        }         
    }

    private void MakeSelectedAttack(){
        targetMinion = TSM.GetMinionUnit(targetPosition);
        pendingAnimations.Add(TSM.SpawnAction(selectedAction, targetMinion.transform.position));
        TSM.StartActionAnimationTimer(pendingAnimations);
        DamageDetails damageDetails = selectedMinion.MakeMinonAttack(selectedAction, targetMinion);
        CheckFaintedMinion(damageDetails.faintedOptions, targetMinion);
    }

    private void CheckFaintedMinion(FaintedOptions faintedOptions, MinionUnit minion){
        if(faintedOptions == FaintedOptions.MinionFainted)
            TSM.RemoveMinionFromBattleground(minion);
        if(faintedOptions == FaintedOptions.TrainerFainted)
            TSM.SetWinner(TSM.GetCurrentPlayerTurn());
        if(CheckOnlyLastTrainer(TSM.GetEnemyTeam()))
            TSM.SetWinner(TSM.GetCurrentPlayerTurn());
        if(CheckOnlyLastTrainer(TSM.GetCurrentPlayerTurn()))
            TSM.SetWinner(TSM.GetEnemyTeam());
    }

    private bool CheckOnlyLastTrainer(Team team){
        List<MinionUnit> lastingMinions = TSM.GetMinionUnitList(team);
        if(lastingMinions.Count == 1) return true;
        return false;
    }

    public override void Exit()
    {
        base.Exit();
        Minion minion = selectedMinion.minion;
        UIManager.Instance.UpdateSelectedFloatingBars(minion.health, minion.MaxHealth(), minion.magic, minion.MaxMagic());
        TSM.UpdateAllMinionUnitGraphics();
        TSM.SetAnimationTimer(AnimationTimer.None);
        pendingAnimations.Clear();
    }
}
