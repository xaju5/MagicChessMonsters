using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinionDisplay : MonoBehaviour
{
    [SerializeField] private Slider HealthBar;

    public Minion minion { get; private set; }
    private Vector2 targetPosition, targetScale;
    
    public void SetUpData(MinionSO minionInfo, Team team){
        minion = new Minion(minionInfo, team);
        GetComponent<SpriteRenderer>().sprite = minionInfo.Sprite;
        HealthBar.GetComponent<FloatingHealthBar>().SetMaxHealthBar(minion.MaxHealth());
    }

}
