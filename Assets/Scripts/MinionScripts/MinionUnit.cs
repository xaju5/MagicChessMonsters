using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinionUnit : MonoBehaviour
{
    [SerializeField] private Slider HealthBar;
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
        HealthBar.GetComponent<FloatingHealthBar>().SetMaxHealthBar(minion.MaxHealth());
    }

    public void MoveMinionUnit(Vector3 targetPosition, bool force = false){
        Debug.Log($"{transform.name} commanded to move");
        if(force)
            transform.position = targetPosition;

        this.targetPosition = targetPosition;
    }

}
