using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    

    [Header("Selected Minion Icon")]
    [SerializeField] private Image selectedMinionIcon;
    [SerializeField] private Slider selectedMinionHealthBar;
    [SerializeField] private Slider selectedMinionMagicBar;

    [Header("Action 1")]
    [SerializeField] private TextMeshProUGUI Action1Text;
    [SerializeField] private TextMeshProUGUI Action1MagicCostText;
    [SerializeField] private TextMeshProUGUI Action1TypeText;
    
    [Header("Action 2")]
    [SerializeField] private TextMeshProUGUI Action2Text;
    [SerializeField] private TextMeshProUGUI Action2MagicCostText;
    [SerializeField] private TextMeshProUGUI Action2TypeText;
    
    [Header("Others")]
    [SerializeField] private TextMeshProUGUI currentTurnText;


    public static UIManager Instance;

    private void Awake()
    {
        SetUpSingleton();
        UpdateTurnText(Team.Player1);
        RemoveSelectedMinionUI();
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
        currentTurnText.text = $"{currentTurn}'s turn";
    }

    public void SetupSelectedMinionUI(Minion selectedMinion){
        selectedMinionIcon.enabled = true;
        selectedMinionHealthBar.gameObject.SetActive(true);
        selectedMinionMagicBar.gameObject.SetActive(true);
        selectedMinionIcon.sprite = selectedMinion.MinionInfo.Sprite;
        selectedMinionHealthBar.value = selectedMinion.health / selectedMinion.MaxHealth();
        selectedMinionMagicBar.value = selectedMinion.magic / selectedMinion.MaxMagic();

        if (selectedMinion.action1 != null)
        {
            Action1Text.transform.parent.gameObject.SetActive(true);
            Action1Text.text = selectedMinion.action1.ActionInfo.Name;
            Action1MagicCostText.text = selectedMinion.action1.MagicCost.ToString();
            Action1TypeText.text = selectedMinion.action1.ActionInfo.Type.ToString();
        }
        if (selectedMinion.action2 != null)
        {
            Action2Text.transform.parent.gameObject.SetActive(true);
            Action2Text.text = selectedMinion.action2.ActionInfo.Name;
            Action2MagicCostText.text = selectedMinion.action2.MagicCost.ToString();
            Action2TypeText.text = selectedMinion.action2.ActionInfo.Type.ToString();
        }
    }

    public void RemoveSelectedMinionUI(){
        selectedMinionIcon.enabled = false;
        selectedMinionIcon.sprite = null;
        selectedMinionHealthBar.gameObject.SetActive(false);
        selectedMinionMagicBar.gameObject.SetActive(false);

        Action1Text.transform.parent.gameObject.SetActive(false);
        Action2Text.transform.parent.gameObject.SetActive(false);
    }

}
