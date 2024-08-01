using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image selectedMinionIcon;
    [SerializeField] private TextMeshProUGUI currentTurnText;

    public static UIManager Instance;

    private void Awake()
    {
        SetUpSingleton();
        UpdateTurnText(Team.Player1);
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

    public void UpdateTurnText(Team currentTurn){
        currentTurnText.text = $"{currentTurn.ToString()}'s turn";
    }

}
