using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image selectedMinionIcon;

    public UIManager Instance;

    private void Awake()
    {
        SetUpSingleton();
    }
    void Update()
    {
        MinionUnit selectedMinion = BattleManager.Instance.GetSelectedMinion();
        if(selectedMinion == null){
            selectedMinionIcon.enabled = false;
            selectedMinionIcon.sprite = null;
            return;
        }
        selectedMinionIcon.enabled = true;
        selectedMinionIcon.sprite = selectedMinion.minion.MinionInfo.Sprite;
    }


    private void SetUpSingleton()
    {
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }


}
