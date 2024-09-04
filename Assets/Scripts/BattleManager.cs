using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [SerializeField] private GameObject minionPrefab, actionPrefab;
    [SerializeField] private MinionList[] team1, team2;
    [SerializeField] private MinionSO[] AllMinionSO;
    [SerializeField] float restoreMagicAmount = 15f;

    private MinionUnit[,] minionUnits;
    private MinionUnit selectedMinion;
    private Action selectedAction;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<Vector2Int> availableAttacks = new List<Vector2Int>();
    private Team currentPlayerTurn, stopedPlayerTurn;
    private List<ActionUnit> pendingAnimations = new List<ActionUnit>();

    private bool isGameover, isGamePaused;

    private void Awake()
    {
        SetUpSingleton();
    }
    void Start()
    {
        SpawnAllMinions();
        PositionAllMinions();
        SetUpTurn();
    }

    void Update() {
        if(!isGameover || !isGamePaused)
            RunTurnLogic();
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
    
    // Set Up Minions
    private void SpawnAllMinions(){
        //TODO: Initial Position Phase
        minionUnits = new MinionUnit[Gameboard.TILE_COUNT_X,Gameboard.TILE_COUNT_Y];

        minionUnits[3,0] = SpawnSingleMinion(AllMinionSO[(int)team1[1]],Team.Player1);
        minionUnits[4,0] = SpawnSingleMinion(AllMinionSO[(int)team1[0]],Team.Player1);
        minionUnits[5,0] = SpawnSingleMinion(AllMinionSO[(int)team1[2]],Team.Player1);

        minionUnits[3,7] = SpawnSingleMinion(AllMinionSO[(int)team2[1]],Team.Player2);
        minionUnits[4,7] = SpawnSingleMinion(AllMinionSO[(int)team2[0]],Team.Player2);
        minionUnits[5,7] = SpawnSingleMinion(AllMinionSO[(int)team2[2]],Team.Player2);
    }
    private MinionUnit SpawnSingleMinion(MinionSO minionInfo, Team team){
        GameObject minionGO = Instantiate(minionPrefab, transform);
        minionGO.name = minionInfo.MinionId.ToString();
        minionGO.GetComponent<SpriteRenderer>().sortingOrder = Gameboard.Instance.GetTilemapRenderer().sortingOrder + 2;

        MinionUnit minionUnit = minionGO.GetComponent<MinionUnit>();
        minionUnit.SetUpData(minionInfo, team);

        return minionUnit;
    }
    private void PositionAllMinions(){
        for (int x = 0; x < Gameboard.TILE_COUNT_X; x++)
            for (int y = 0; y < Gameboard.TILE_COUNT_Y; y++)
                minionUnits[x,y]?.MoveMinionUnit(new Vector2Int(x,y), true);
    } 
    private void SetUpTurn()
    {
        currentPlayerTurn = Team.Player1;
        isGameover = false;
        isGamePaused = false;
        pendingAnimations.Clear();
    }
    //Turn Logic
    private void RunTurnLogic(){
        Vector2Int currentHover = Gameboard.Instance.GetCurrentHover();
        if(selectedMinion){
            if(Input.GetMouseButtonDown(0)  && !isGamePaused){
                if(currentHover == -Vector2Int.one)
                    DeselectMinion();
                else{
                    if(minionUnits[currentHover.x, currentHover.y] == null){
                        if(IsValidMove(currentHover)){
                            MoveSelectedMinionTo(currentHover);
                            FinishTurn();
                        }
                    }
                    else if(minionUnits[currentHover.x, currentHover.y].Team == currentPlayerTurn){
                        SelectMinion(currentHover);   
                    }
                    else if(minionUnits[currentHover.x, currentHover.y].Team != currentPlayerTurn){
                        if(IsValidAttack(currentHover)){
                            MakeSelectedAttack(currentHover);
                        }
                    }
                }
            }
            if(Input.GetKeyDown(KeyCode.Q)  && !isGamePaused)
                SelectAction(selectedMinion.minion.action1);
            
            if(Input.GetKeyDown(KeyCode.W)  && !isGamePaused)
                SelectAction(selectedMinion.minion.action2);
        }
        else{
            if(availableMoves.Count < 1){
                availableMoves = GetTeamMinionPositions(currentPlayerTurn);
                Gameboard.Instance.ChangeTilesLayers(availableMoves,"Highlight");
            }
            if(CanMinionBeSelected(currentHover))
                SelectMinion(currentHover);    
        }    
    }
    private bool CanMinionBeSelected(Vector2Int currentHover)
    {
        if(
            Input.GetMouseButtonDown(0) &&
            currentHover != -Vector2Int.one &&
            minionUnits[currentHover.x, currentHover.y] != null &&
            minionUnits[currentHover.x, currentHover.y]?.Team == currentPlayerTurn
            )
            return true;

        return false;
    }
    private void DeselectMinion(){
        DeselectAction();
        selectedMinion = null;
        UIManager.Instance.RemoveSelectedMinionUI();
        Gameboard.Instance.ChangeTilesLayers(availableMoves,"Tile");
        availableMoves.Clear();
    }
    private void SelectMinion(Vector2Int tileIndex){
        DeselectMinion();
        selectedMinion = minionUnits[tileIndex.x,tileIndex.y];
        UIManager.Instance.SetupSelectedMinionUI(selectedMinion.minion);
        availableMoves = selectedMinion.minion.GetAvailableMoves(ref minionUnits, tileIndex, Gameboard.TILE_COUNT_X, Gameboard.TILE_COUNT_Y);
        Gameboard.Instance.ChangeTilesLayers(availableMoves,"Highlight");
    }
    private void DeselectAction(){
        selectedAction = null;
        Gameboard.Instance.ChangeTilesLayers(availableAttacks,"Tile");
        availableAttacks.Clear();
    }

    private void SelectAction(Action action){
        DeselectAction();
        selectedAction = action;
        availableAttacks = selectedAction.GetAvailableAttackTiles(ref minionUnits, selectedMinion.MinionIndex, Gameboard.TILE_COUNT_X, Gameboard.TILE_COUNT_Y, GetEnemyTeamName());
        Gameboard.Instance.ChangeTilesLayers(availableAttacks,"Danger");
    }   
    private void MoveSelectedMinionTo(Vector2Int newPosition){
        Vector2Int currentPosition = LookupMinionIndex(selectedMinion);
        if(currentPosition == newPosition)
            return;

        minionUnits[currentPosition.x, currentPosition.y] = null;
        minionUnits[newPosition.x, newPosition.y] = selectedMinion;
        selectedMinion.MoveMinionUnit(new Vector2Int(newPosition.x,newPosition.y));
    }
    private Vector2Int LookupMinionIndex(MinionUnit minion){
        for (int x = 0; x < Gameboard.TILE_COUNT_X; x++)
            for (int y = 0; y < Gameboard.TILE_COUNT_Y; y++)
                if(minionUnits[x,y] == minion)
                    return new Vector2Int(x,y);

        throw new System.Exception("LookupMinionIndex_NotFound");
        // return -Vector2Int.one;
    }
    private List<Vector2Int> GetTeamMinionPositions(Team team){
        List<Vector2Int> minionPositions = new List<Vector2Int>();
        for (int x = 0; x < Gameboard.TILE_COUNT_X; x++)
            for (int y = 0; y < Gameboard.TILE_COUNT_Y; y++)
                if(minionUnits[x,y]?.Team == team)
                    minionPositions.Add(new Vector2Int(x,y));
        return minionPositions;
    }
    private List<MinionUnit> GetTeamMinions(Team team){
        List<MinionUnit> minions = new List<MinionUnit>();
        for (int x = 0; x < Gameboard.TILE_COUNT_X; x++)
            for (int y = 0; y < Gameboard.TILE_COUNT_Y; y++)
                if(minionUnits[x,y]?.Team == team)
                    minions.Add(minionUnits[x,y]);
        return minions;
    }
    private void MakeSelectedAttack(Vector2Int currentHover){
        if (!selectedMinion.canMakeAttack(selectedAction)) return;
        MinionUnit targetMinion = minionUnits[currentHover.x, currentHover.y];
        pendingAnimations.Add(SpawnAction(selectedAction, targetMinion.transform.position));
        StartCoroutine(WaitForAnimationsAndFinishTurn(selectedMinion, selectedAction, targetMinion));
        StopTurn();
    }

    private ActionUnit SpawnAction(Action action, Vector3 targetPosition){
        float movementAngle = MathUtils.GetVectorAngle(selectedMinion.transform.position - targetPosition); //TODO: Rotar ataque hacia enemigo
        GameObject actionGO = Instantiate(actionPrefab, selectedMinion.transform.position, new Quaternion());
        actionGO.name = action.ActionInfo.Name;
        actionGO.GetComponent<SpriteRenderer>().sortingOrder = Gameboard.Instance.GetTilemapRenderer().sortingOrder + 3;
        ActionUnit actionUnit = actionGO.GetComponent<ActionUnit>();
        actionUnit.SetUpData(action, targetPosition);
        return actionUnit;
    }

    private IEnumerator WaitForAnimationsAndFinishTurn(MinionUnit attackerMinion, Action action, MinionUnit targetMinion){
        yield return StartCoroutine(WaitForActionAnimationEnd());
        restoreTurn(attackerMinion);
        DamageDetails damageDetails = attackerMinion.MakeMinonAttack(action, targetMinion);
        Minion minion = attackerMinion. minion;
        UIManager.Instance.UpdateSelectedFloatingBars(minion.health, minion.MaxHealth(), minion.magic, minion.MaxMagic());
        CheckFaintedMinion(damageDetails.faintedOptions, targetMinion);
        FinishTurn();
    }

    private IEnumerator WaitForActionAnimationEnd(){
        while (pendingAnimations.Count > 0)
        {
            for (int i = pendingAnimations.Count - 1; i >= 0; i--)
            {
                if(pendingAnimations[i].HasAnimationFinished()){
                    Destroy(pendingAnimations[i].gameObject);
                    pendingAnimations.RemoveAt(i);
                }
            }
            yield return null;
        }

    }
    private void CheckFaintedMinion(FaintedOptions faintedOptions, MinionUnit minion){
        if(faintedOptions == FaintedOptions.MinionFainted) 
            RemoveMinionFromBattleground(minion);
        if(faintedOptions == FaintedOptions.TrainerFainted)
            SetWinner(currentPlayerTurn);
        if(CheckOnlyLastTrainer(GetEnemyTeamName()))
            SetWinner(currentPlayerTurn);
        if(CheckOnlyLastTrainer(currentPlayerTurn))
            SetWinner(GetEnemyTeamName());
    }
    private void RemoveMinionFromBattleground(MinionUnit targetMinion, bool force = false)
    {
        if(force) Destroy(targetMinion.gameObject);
        minionUnits[targetMinion.MinionIndex.x,targetMinion.MinionIndex.y] = null;
    }

    private bool CheckOnlyLastTrainer(Team team){
        List<MinionUnit> lastingMinions = GetTeamMinions(team);
        if(lastingMinions.Count == 1) return true;
        return false;
    }

    private void SetWinner(Team currentPlayerTurn)
    {
        isGameover = true;
        UIManager.Instance.SetupWinnerScreen(currentPlayerTurn);
    }

    private void FinishTurn(){
        if(currentPlayerTurn == Team.None) currentPlayerTurn = stopedPlayerTurn;
        RefreshUnselectedMinionMagic();
        DeselectMinion();
        if(isGameover) return;
        currentPlayerTurn = GetEnemyTeamName();
        UIManager.Instance.UpdateTurnText(currentPlayerTurn);
    }

    private void StopTurn(){
        DeselectMinion();
        stopedPlayerTurn = currentPlayerTurn;
        currentPlayerTurn = Team.None;
    } 
    private void restoreTurn(MinionUnit minion){
        currentPlayerTurn = stopedPlayerTurn;
        selectedMinion = minion;
    } 

    private Team GetEnemyTeamName(){
        if(currentPlayerTurn == Team.Player1)
            return Team.Player2;
        else
            return Team.Player1;
    }

    private void RefreshUnselectedMinionMagic(){

        foreach (MinionUnit minionUnit in GetTeamMinions(currentPlayerTurn))
        {
            if(minionUnit != selectedMinion) minionUnit.RestoreMagic(restoreMagicAmount);
        }
    }

    private void DestroyAllMinions(){
        for (int x = 0; x < Gameboard.TILE_COUNT_X; x++)
            for (int y = 0; y < Gameboard.TILE_COUNT_Y; y++)
                if(minionUnits[x,y] != null)
                    RemoveMinionFromBattleground(minionUnits[x,y], true);
    }

    //Public methods
    public MinionUnit GetSelectedMinion(){
        return selectedMinion;
    }

    public bool IsValidMove(Vector2Int index){
        foreach (Vector2Int availableIndex in availableMoves){
            if(availableIndex == index){
                return true;
            }
        }
        return false;
    }

    public bool IsValidAttack(Vector2Int index){
        foreach (Vector2Int availableIndex in availableAttacks){
            if(availableIndex == index){
                return true;
            }
        }
        return false;
    }

    public void ResetGame(){
        UIManager.Instance.Resume();
        DeselectMinion();
        DestroyAllMinions();
        SpawnAllMinions();
        PositionAllMinions();
        SetUpTurn();
        UIManager.Instance.RemoveWinnerScreen();
    }

    public void PauseGame(bool isGamePaused){
        this.isGamePaused = isGamePaused;
    }
}