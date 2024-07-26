using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minion : MonoBehaviour
{
    [SerializeField] private float minionSize = 1f;
    [SerializeField] Slider HealthBar;
    [SerializeField] private bool isTrainer = false;

    [SerializeField] private MinionSO minionInfo;
    [SerializeField] private Team team;
    
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = minionInfo.Sprite;
        HealthBar.GetComponent<FloatingHealthBar>().SetMaxHealthBar(minionInfo.HealthBase);
    }

    void Update()
    {
        
    }
}
