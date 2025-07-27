using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonState : BaseState
{
    private TurnStateMachine TSM;
    private MinionUnit selectedMinion;
    private MinionUnit targetMinion;
    private Action selectedAction;
    private Vector2Int targetPosition;
    private List<ActionUnit> pendingAnimations = new List<ActionUnit>();
    public SummonState(TurnStateMachine stateMachine) : base("Summon", stateMachine)
    {
        TSM = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        // selectedMinion = TSM.GetSelectedMinion();
        // selectedAction = TSM.GetSelectedAction();
        // targetPosition = TSM.GetTargetPosition();
        // targetMinion = TSM.GetMinionUnit(targetPosition);
        // TSM.SetAnimationTimer(AnimationTimer.None);
        // selectedMinion.SetCanTalk(false);      
        // targetMinion.SetCanTalk(false);      
        // MakeSelectedAttack(); 
    }

    public override void Update()
    {
        base.Update();
        stateMachine.ChangeState(TSM.endTurnState);
        // if (TSM.GetAnimationTimer() == AnimationTimer.Finished) {
        //     selectedMinion.SetCanTalk(true);
        //     targetMinion.SetCanTalk(true);
        //     if (TSM.GetWinner() != Team.None)
        //     {
        //         stateMachine.ChangeState(TSM.gameoverState);
        //         return;
        //     }
        //     stateMachine.ChangeState(TSM.endTurnState);
        //     return;
        // }       
    }

    public override void Exit()
    {
        base.Exit();
        // Minion minion = selectedMinion.minion;
        // UIManager.Instance.UpdateSelectedFloatingBars(minion.health, minion.MaxHealth(), minion.magic, minion.MaxMagic());
        // TSM.UpdateAllMinionUnitGraphics();
        // TSM.SetAnimationTimer(AnimationTimer.None);
        // pendingAnimations.Clear();
    }
}
