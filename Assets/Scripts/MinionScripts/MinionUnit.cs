using System.Collections;
using TMPro;
using UnityEngine;

public class MinionUnit : MonoBehaviour
{
    [SerializeField] private FloatingBar HealthBar;
    [SerializeField] private FloatingBar MagicBar;
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float dialogueboxDuration = 2f;
    [SerializeField] private TextMeshProUGUI dialogueText;
    public Minion minion { get; private set; }
    public Team Team { get; private set; }
    public bool IsTrainer { get; private set; }
    public Vector2Int MinionIndex { get; private set; }
    private Vector3 targetPosition;

    Animator animator;
    
    void Update() {
        if(transform.position != targetPosition)
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movementSpeed);
    }

    public void SetUpData(MinionSO minionInfo, Team team){
        Team = team;
        IsTrainer = (minionInfo.MinionId == MinionList.Boy) || (minionInfo.MinionId == MinionList.Girl) ? true : false;
        
        minion = new Minion(minionInfo);
        GetComponent<SpriteRenderer>().sprite = minionInfo.Sprite;
        HealthBar.SetBarMaxValue(minion.MaxHealth());
        MagicBar.SetBarMaxValue(minion.MaxMagic());

        SetUpAnimationController(minionInfo);
    }

    private void SetUpAnimationController(MinionSO minionInfo){
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = minionInfo.Animator;
        animator.SetBool("hasFainted",false);
    }

    private void UpdateFloatingBars(){
        HealthBar.UpdateBarValue(minion.health);
        MagicBar.UpdateBarValue(minion.magic);
    }

    private IEnumerator TriggerMinionDead(){
        animator.SetBool("hasFainted",true);
        while(!HasAnimationFinished("DeadState")){
            yield return null;
        }
        gameObject.SetActive(false);
    }

    private bool HasAnimationFinished(string animationName){
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }

    private IEnumerator WriteMessageInDialogueBox(string message){
        dialogueText.transform.parent.gameObject.SetActive(true);
        dialogueText.text = message;
        yield return new WaitForSeconds(dialogueboxDuration);
        dialogueText.transform.parent.gameObject.SetActive(false);
    }

    public void MoveMinionUnit(Vector2Int newMinionIndex, bool force = false){
        MinionIndex = newMinionIndex;
        targetPosition = Gameboard.Instance.GetTileCenter(newMinionIndex.x,newMinionIndex.y);
        if(force)
            transform.position = targetPosition;
    }

    public FaintedOptions TakeDamage(Action attackerAction, MinionSO attacker)
    {
        bool isFainted = minion.TakeDamage(attackerAction, attacker);
        UpdateFloatingBars();
        if (isFainted)
        {
            StartCoroutine(TriggerMinionDead());
            return IsTrainer ? FaintedOptions.TrainerFainted : FaintedOptions.MinionFainted;
        }
        StartCoroutine(WriteMessageInDialogueBox("Augh!"));
        return FaintedOptions.None;
    }

    public bool Heal(float amount){
        bool isFullyHealed = minion.Heal(amount);
        UpdateFloatingBars();
        return isFullyHealed;
    }
    public bool ConsumeMagic(float amount){
        bool isMagicDrained = minion.ConsumeMagic(amount);
        UpdateFloatingBars();
        return isMagicDrained;
    }
    public bool RestoreMagic(float amount){
        bool isMagicFull = minion.RestoreMagic(amount);
        UpdateFloatingBars();
        return isMagicFull;
    }

}
