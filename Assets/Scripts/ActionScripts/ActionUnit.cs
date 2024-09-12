using UnityEngine;

public class ActionUnit : MonoBehaviour
{
    public Action action { get; private set; }
    private Animator animator;
    private Vector3 targetPosition;

    public void SetUpData(Action action, Vector3 targetPosition){
        this.action = action;
        this.targetPosition = targetPosition;
        SetUpAnimationController(action.ActionInfo);
    }

    private void SetUpAnimationController(ActionSO actionInfo){
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = actionInfo.Animator;
        animator.SetBool("hasHit",false);
    }

    void Update() {
        if((transform.position - targetPosition).sqrMagnitude >= 0.01){
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * action.ActionInfo.AnimationTravelSpeed * 3);
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
            animator.SetBool("hasHit",true);
        }
    }
    public bool HasAnimationFinished(string animationStateName = "HitState"){
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && animator.GetCurrentAnimatorStateInfo(0).IsName(animationStateName);
    }
}
