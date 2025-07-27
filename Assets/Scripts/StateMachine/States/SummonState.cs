using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonState : BaseState
{
    private TurnStateMachine TSM;
    private MinionUnit minionToSummon;
    private MinionUnit selectedMinion;
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
        targetPosition = TSM.GetTargetPosition();
        minionToSummon = TSM.GetMinionToSummon();
        selectedMinion = TSM.GetSelectedMinion();
        selectedAction = TSM.GetSelectedAction();
        TSM.SetAnimationTimer(AnimationTimer.None);
        pendingAnimations.Add(TSM.SpawnAction(selectedAction, Gameboard.Instance.GetTileCenter(targetPosition.x, targetPosition.y)));
        TSM.StartActionAnimationTimer(pendingAnimations);
    }

    public override void Update()
    {
        base.Update();
        if (TSM.GetAnimationTimer() == AnimationTimer.Finished)
        {
            SummonMinion();
            stateMachine.ChangeState(TSM.endTurnState);
            return;
        }
    }
    private void SummonMinion()
    {
        TSM.SummonMinion(minionToSummon, targetPosition);
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
