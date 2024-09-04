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
    
    [Header("Winner Screen")]
    [SerializeField] private TextMeshProUGUI winnerText;
    [Header("Pause Screen")]
    [SerializeField] private GameObject pauseMenuUI;


    [Header("Others")]
    [SerializeField] private TextMeshProUGUI currentTurnText;


    public static UIManager Instance;
    public static bool gameIsPaused = false;
    public static bool isGameover = false;

    private void Awake()
    {
        SetUpSingleton();
        UpdateTurnText(Team.Player1);
        RemoveSelectedMinionUI();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameover)
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
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

    private void SetUpSliderData(Slider slider, float maxAmount, float amount){
        slider.gameObject.SetActive(true);
        FloatingBar floatingBar = slider.GetComponent<FloatingBar>();
        floatingBar.SetBarMaxValue(maxAmount);
        floatingBar.ForceBarValue(amount);
    }
    private void UpdateSliderData(Slider slider, float maxAmount, float amount){
        FloatingBar floatingBar = slider.GetComponent<FloatingBar>();
        floatingBar.UpdateBarValue(amount);
    }

    private void SetUpActionData(Action selectedMinionAction, TextMeshProUGUI actionName, TextMeshProUGUI actionCost, TextMeshProUGUI actionType){
        actionName.transform.parent.gameObject.SetActive(true);
        actionName.text = selectedMinionAction.ActionInfo.Name;
        actionCost.text = selectedMinionAction.MagicCost.ToString();
        actionType.text = selectedMinionAction.ActionInfo.Type.ToString();
    }

    public void UpdateTurnText(Team currentTurn){
        currentTurnText.text = $"{currentTurn}'s turn";
    }

    public void SetupSelectedMinionUI(Minion selectedMinion){
        selectedMinionIcon.enabled = true;
        selectedMinionIcon.sprite = selectedMinion.MinionInfo.Sprite;
        SetUpSliderData(selectedMinionHealthBar, selectedMinion.MaxHealth(),selectedMinion.health);
        selectedMinionHealthBarText.text = $"{selectedMinion.health}/{selectedMinion.MaxHealth()}";
        SetUpSliderData(selectedMinionMagicBar, selectedMinion.MaxMagic(),selectedMinion.magic);
        selectedMinionMagicBarText.text = $"{selectedMinion.magic}/{selectedMinion.MaxMagic()}";

        if (selectedMinion.action1 != null)
            SetUpActionData(selectedMinion.action1, Action1Text, Action1MagicCostText, Action1TypeText);
        if (selectedMinion.action2 != null)
            SetUpActionData(selectedMinion.action2, Action2Text, Action2MagicCostText, Action2TypeText);        
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

    public void SetupWinnerScreen(Team winner){
        isGameover = true;
        RemoveSelectedMinionUI();
        currentTurnText.enabled = false;
        winnerText.transform.parent.gameObject.SetActive(true);
        winnerText.text = winner.ToString(); 
    }

    //PauseMenu Methods
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }


    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

}
