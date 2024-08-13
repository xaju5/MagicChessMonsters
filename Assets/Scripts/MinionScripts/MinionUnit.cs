using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinionUnit : MonoBehaviour
{
    [SerializeField] private FloatingBar HealthBar;
    [SerializeField] private FloatingBar MagicBar;
    [SerializeField] private float movementSpeed = 10f;
    public Minion minion { get; private set; }
    public Team Team { get; private set; }
    public bool IsTrainer { get; private set; }
    private Vector3 targetPosition, targetScale;
    
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
    }

    private void UpdateFloatingBars(){
        HealthBar.UpdateBarValue(minion.health);
        MagicBar.UpdateBarValue(minion.magic);
    }

    public void MoveMinionUnit(Vector3 targetPosition, bool force = false){
        if(force)
            transform.position = targetPosition;

        this.targetPosition = targetPosition;
    }

    public bool TakeDamage(Action attackerAction, MinionSO attacker){
        bool isFainted = minion.TakeDamage(attackerAction, attacker);
        UpdateFloatingBars();
        return isFainted;
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
