using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    private TurnStateMachine TSM;
    private Vector2Int currentHover;
    private List<Vector2Int> availableAttacks;
    private MinionUnit selectedMinion;
    private MinionUnit targetMinion;
    private Action selectedAction;
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
        availableAttacks = selectedAction.GetAvailableAttackTiles(ref TSM.GetMinionUnitsArray(), selectedMinion.MinionIndex, Gameboard.TILE_COUNT_X, Gameboard.TILE_COUNT_Y, TSM.GetEnemyTeam());
        Gameboard.Instance.ChangeTilesLayers(availableAttacks,TileLayer.Danger);
        TSM.SetAnimationTimer(AnimationTimer.None);
    }

    public override void Update()
    {
        base.Update();
        currentHover = Gameboard.Instance.GetCurrentHover();
        switch (TSM.GetAnimationTimer())
        {   
            case AnimationTimer.None:
                if(availableAttacks.Count == 0){
                    selectedMinion.QueueMessage("Enemy out of range!");
                    stateMachine.ChangeState(TSM.moveState);
                    return;
                }
                if (!selectedMinion.canMakeAttack(selectedAction)){
                    stateMachine.ChangeState(TSM.moveState);
                    return;
                }    
                if(!Input.GetMouseButtonDown(0)) return;
                if(currentHover == -Vector2Int.one){
                    stateMachine.ChangeState(TSM.moveState);
                    return;
                } 
                if(!IsValidAttack(currentHover)){
                    stateMachine.ChangeState(TSM.moveState);
                    return;
                } 
                if(TSM.GetMinionUnit(currentHover) == null) return;
                if(TSM.GetMinionUnit(currentHover).Team == TSM.GetCurrentPlayerTurn()) return;
                
                MakeSelectedAttack(currentHover);  
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

    private bool IsValidAttack(Vector2Int index){
        foreach (Vector2Int availableIndex in availableAttacks){
            if(availableIndex == index){
                return true;
            }
        }
        return false;
    }

    private void MakeSelectedAttack(Vector2Int currentHover){
        targetMinion = TSM.GetMinionUnit(currentHover);
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
        Debug.Log($"Winner: {TSM.GetWinner()}");
        Gameboard.Instance.RestoreTilesLayers(availableAttacks);
        Minion minion = selectedMinion.minion;
        UIManager.Instance.UpdateSelectedFloatingBars(minion.health, minion.MaxHealth(), minion.magic, minion.MaxMagic());
        TSM.UpdateAllMinionUnitGraphics();
        TSM.DeselectAction();
        TSM.SetAnimationTimer(AnimationTimer.None);
        pendingAnimations.Clear();
        availableAttacks.Clear();
    }
}
