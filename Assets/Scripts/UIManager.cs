using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Selected Minion Icon")]
    [SerializeField] private Image selectedMinionIcon;
    [SerializeField] private Slider selectedMinionHealthBar;
    [SerializeField] private TextMeshProUGUI selectedMinionHealthBarText;
    [SerializeField] private Slider selectedMinionMagicBar;
    [SerializeField] private TextMeshProUGUI selectedMinionMagicBarText;

    [Header("Action 1")]
    [SerializeField] private TextMeshProUGUI Action1Text;
    [SerializeField] private TextMeshProUGUI Action1MagicCostText;
    [SerializeField] private TextMeshProUGUI Action1TypeText;
    
    [Header("Action 2")]
    [SerializeField] private TextMeshProUGUI Action2Text;
    [SerializeField] private TextMeshProUGUI Action2MagicCostText;
    [SerializeField] private TextMeshProUGUI Action2TypeText;
    
    [Header("Summon Minion")]
    [SerializeField] private GameObject[] summonButton;
    [Header("Winner Screen")]
    [SerializeField] private TextMeshProUGUI winnerText;
    [Header("Pause Screen")]
    [SerializeField] private GameObject pauseMenuUI;


    [Header("Others")]
    [SerializeField] private TextMeshProUGUI currentTurnText;

    public static UIManager Instance;
    public UnityEvent resumeEvent, resetEvent, closeSummonEvent, summonEvent;

    private void Awake()
    {
        SetUpSingleton();
        RemoveSelectedMinionUI();
        UpdateTurnText(Team.Player1);
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

    //Selected Minion UI
    public void SetupSelectedMinionUI(Minion selectedMinion)
    {
        selectedMinionIcon.enabled = true;
        selectedMinionIcon.sprite = selectedMinion.MinionInfo.Sprite;
        SetUpSliderData(selectedMinionHealthBar, selectedMinion.MaxHealth(), selectedMinion.health);
        selectedMinionHealthBarText.text = $"{selectedMinion.health}/{selectedMinion.MaxHealth()}";
        SetUpSliderData(selectedMinionMagicBar, selectedMinion.MaxMagic(), selectedMinion.magic);
        selectedMinionMagicBarText.text = $"{selectedMinion.magic}/{selectedMinion.MaxMagic()}";

        if (selectedMinion.action1 != null)
            SetUpActionData(selectedMinion.action1, Action1Text, Action1MagicCostText, Action1TypeText);
        if (selectedMinion.action2 != null)
            SetUpActionData(selectedMinion.action2, Action2Text, Action2MagicCostText, Action2TypeText);
    }
    private void SetUpSliderData(Slider slider, float maxAmount, float amount){
        slider.gameObject.SetActive(true);
        FloatingBar floatingBar = slider.GetComponent<FloatingBar>();
        floatingBar.SetBarMaxValue(maxAmount);
        floatingBar.ForceBarValue(amount);
    }
    
    private void SetUpActionData(Action selectedMinionAction, TextMeshProUGUI actionName, TextMeshProUGUI actionCost, TextMeshProUGUI actionType)
    {
        actionName.transform.parent.gameObject.SetActive(true);
        actionName.text = selectedMinionAction.ActionInfo.Name;
        actionCost.text = selectedMinionAction.MagicCost.ToString();
        actionType.text = selectedMinionAction.ActionInfo.Type.ToString();
    }

    public void RemoveSelectedMinionUI(){
        selectedMinionIcon.enabled = false;
        selectedMinionIcon.sprite = null;
        selectedMinionHealthBar.gameObject.SetActive(false);
        selectedMinionMagicBar.gameObject.SetActive(false);

        Action1Text.transform.parent.gameObject.SetActive(false);
        Action2Text.transform.parent.gameObject.SetActive(false);
    }

    public void UpdateSelectedFloatingBars(float health, float maxHealth, float magic, float maxMagic){
        UpdateSliderData(selectedMinionHealthBar, maxHealth, health);
        UpdateSliderData(selectedMinionMagicBar, maxMagic, magic);
        selectedMinionHealthBarText.text = $"{health}/{maxHealth}";
        selectedMinionMagicBarText.text = $"{magic}/{maxMagic}";
    }
    private void UpdateSliderData(Slider slider, float maxAmount, float amount){
        FloatingBar floatingBar = slider.GetComponent<FloatingBar>();
        floatingBar.UpdateBarValue(amount);
    }

    //Summon Minion UI
    public void SetupSummonMinionUI(List<MinionUnit> summonableTeamMinions)
    {
        summonButton[0].transform.parent.gameObject.SetActive(true);
        for (int i = 1; i < summonableTeamMinions.Count; i++)
        {
            summonButton[i - 1].SetActive(true);
            summonButton[i - 1].GetComponentInChildren<TextMeshProUGUI>().text = summonableTeamMinions[i].minion.MinionInfo.Type.ToString();
            summonButton[i - 1].GetComponent<Image>().sprite = summonableTeamMinions[i].minion.MinionInfo.Sprite;
        }
    }

    public void DisableSummonMinionUI()
    {
        summonButton[0].transform.parent.gameObject.SetActive(false);
        foreach (GameObject button in summonButton)
        {
            button.SetActive(false);
        }
    }

    //Game Over Methods
    public void EnableWinnerMenu(bool enabled, Team winner = Team.None)
    {
        currentTurnText.enabled = !enabled;
        winnerText.text = winner.ToString();
        winnerText.transform.parent.gameObject.SetActive(enabled);
    }

    //PauseMenu Methods
    public void EnablePauseMenu(bool enabled)
    {
        pauseMenuUI.SetActive(enabled);
    }

    //Buttons
    public void ResumeButton()
    {
        Debug.Log("Resume button pressed.");
        resumeEvent.Invoke();
    }

    public void ResetButton()
    {
        Debug.Log("Reset button pressed.");
        resetEvent.Invoke();
    }
    public void CloseSummonButton()
    {
        Debug.Log("Close Summon button pressed.");
        closeSummonEvent.Invoke();
    }
    public void SummonButton()
    {
        Debug.Log("Close Summon button pressed.");
        summonEvent.Invoke();
    }
    
    public void QuitGameButton()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

}
