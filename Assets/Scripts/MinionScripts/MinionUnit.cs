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

    public DamageDetails TakeDamage(Action attackerAction, MinionSO attacker)
    {
        DamageDetails damageDetails = minion.TakeDamage(attackerAction, attacker);
        UpdateFloatingBars();
        if (damageDetails.isFainted)
        {
            StartCoroutine(TriggerMinionDead());
            damageDetails.faintedOptions = IsTrainer ? FaintedOptions.TrainerFainted : FaintedOptions.MinionFainted;
        }
        else
        {
            StartCoroutine(WriteMessageInDialogueBox("-"+damageDetails.total_damage.ToString()));
            damageDetails.faintedOptions = FaintedOptions.None;
        }

        return damageDetails;
    }

    public DamageDetails MakeMinonAttack(Action selectedAction, MinionUnit targetMinion){
        if(!canMakeAttack(selectedAction)) {
            StartCoroutine(WriteMessageInDialogueBox("I need Magic!"));
            DamageDetails nullDetails = new DamageDetails(false,0,0);
            nullDetails.faintedOptions = FaintedOptions.Invalid;
            return nullDetails;
        }
        ConsumeMagic(selectedAction.MagicCost);
        DamageDetails damageDetails = targetMinion.TakeDamage(selectedAction, minion.MinionInfo);
        string text = GetAttackerMessage(damageDetails);
        if(text!= "") StartCoroutine(WriteMessageInDialogueBox(text));
        return damageDetails;

    }
    private bool canMakeAttack(Action selectedAction)
    {
        return selectedAction.MagicCost <= minion.magic;
    }
    private string GetAttackerMessage(DamageDetails damageDetails)
    {
        string text = "";
        if(damageDetails.typeEffectivines == 0 ){
            text = "It doesnt affect";
            return text;
        }
        else if(damageDetails.typeEffectivines == 2){
            text = "It's super effective";
        }
        else if(damageDetails.typeEffectivines == 0.5){
            text = "It's not very effective...";
        }
        if(damageDetails.isCritical)
            text += "Critical!";

        return text;
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
