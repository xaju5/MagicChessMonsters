using System.Collections;
using System.Collections.Generic;
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
    private List<string> pendingMessages;

    private bool canTalk;
    Animator animator;

    void Update()
    {
        if (transform.position != targetPosition)
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movementSpeed);
        if (pendingMessages.Count > 0 && canTalk)
            StartCoroutine(WriteMessagesInDialogueBox(pendingMessages));

    }

    public void SetUpData(MinionSO minionInfo, Team team, Vector2Int initialIndex)
    {
        Team = team;
        IsTrainer = (minionInfo.MinionId == MinionList.Boy) || (minionInfo.MinionId == MinionList.Girl) ? true : false;

        minion = new Minion(minionInfo);
        GetComponent<SpriteRenderer>().sprite = minionInfo.Sprite;
        HealthBar.SetBarMaxValue(minion.MaxHealth());
        MagicBar.SetBarMaxValue(minion.MaxMagic());
        pendingMessages = new List<string>();
        canTalk = true;

        SetUpAnimationController(minionInfo);
        MoveMinionUnit(initialIndex, true);
    }

    private void SetUpAnimationController(MinionSO minionInfo)
    {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = minionInfo.Animator;
        animator.SetBool("hasFainted", false);
    }

    private void UpdateFloatingBars()
    {
        HealthBar.UpdateBarValue(minion.health);
        MagicBar.UpdateBarValue(minion.magic);
    }

    private void EnableFloatingBars(bool enable)
    {
        HealthBar.gameObject.SetActive(enable);
        MagicBar.gameObject.SetActive(enable);
    }

    private IEnumerator TriggerMinionDead()
    {
        while (pendingMessages.Count > 0)
        {
            yield return null;
        }
        animator.SetBool("hasFainted", true);
        while (!HasAnimationFinished("DeadState"))
        {
            yield return null;
        }
        // Destroy(gameObject);

    }

    private bool HasAnimationFinished(string animationName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }

    private IEnumerator WriteMessagesInDialogueBox(List<string> pendingMessages)
    {
        while (pendingMessages.Count > 0)
        {
            dialogueText.transform.parent.gameObject.SetActive(true);
            dialogueText.text = pendingMessages[0];
            pendingMessages.RemoveAt(0);
            yield return new WaitForSeconds(dialogueboxDuration);
            dialogueText.transform.parent.gameObject.SetActive(false);
        }
    }

    public void MoveMinionUnit(Vector2Int newMinionIndex, bool force = false)
    {
        MinionIndex = newMinionIndex;
        targetPosition = Gameboard.Instance.GetTileCenter(newMinionIndex.x, newMinionIndex.y);
        if (force)
            transform.position = targetPosition;
    }

    public DamageDetails MakeMinonAttack(Action selectedAction, MinionUnit targetMinion)
    {
        ConsumeMagic(selectedAction.MagicCost);
        DamageDetails damageDetails = targetMinion.TakeDamage(selectedAction, minion.MinionInfo);
        string text = GetAttackerMessage(damageDetails);
        if (text != "") pendingMessages.Add(text);
        return damageDetails;
    }

    public DamageDetails TakeDamage(Action attackerAction, MinionSO attacker)
    {
        DamageDetails damageDetails = minion.TakeDamage(attackerAction, attacker);
        if (damageDetails.isFainted)
        {
            damageDetails.faintedOptions = IsTrainer ? FaintedOptions.TrainerFainted : FaintedOptions.MinionFainted;
        }
        else
        {
            pendingMessages.Add("-" + damageDetails.total_damage.ToString());
            damageDetails.faintedOptions = FaintedOptions.None;
        }

        return damageDetails;
    }

    public bool HasEnoughMagic(Action selectedAction)
    {
        bool canMakeAttack = selectedAction.MagicCost <= minion.magic;
        if (!canMakeAttack) pendingMessages.Add("I need Magic!");
        return canMakeAttack;
    }
    private string GetAttackerMessage(DamageDetails damageDetails)
    {
        string text = "";
        if (damageDetails.typeEffectivines == 0)
        {
            text = "It doesnt affect";
            return text;
        }
        else if (damageDetails.typeEffectivines == 2)
        {
            text = "It's super effective";
        }
        else if (damageDetails.typeEffectivines == 0.5)
        {
            text = "It's not very effective...";
        }
        if (damageDetails.isCritical)
            text += "Critical!";

        return text;
    }

    public bool Heal(float amount)
    {
        bool isFullyHealed = minion.Heal(amount);
        return isFullyHealed;
    }
    public bool ConsumeMagic(float amount)
    {
        bool isMagicDrained = minion.ConsumeMagic(amount);
        return isMagicDrained;
    }
    public bool RestoreMagic(float amount)
    {
        bool isMagicFull = minion.RestoreMagic(amount);
        return isMagicFull;
    }

    public void UpdateMinionUnitGraphics()
    {
        UpdateFloatingBars();
        if (minion.IsFainted())
        {
            EnableFloatingBars(false);
            StartCoroutine(TriggerMinionDead());
        }

    }

    public void QueueMessage(string text)
    {
        pendingMessages.Add(text);
    }

    public void SetCanTalk(bool enable)
    {
        canTalk = enable;
    }

}
