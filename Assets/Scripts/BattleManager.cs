using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private MinionList[] team1, team2;
    [SerializeField] private MinionSO[] AllMinionSO;

    private MinionUnit[,] minionUnits;
    private MinionUnit selectedMinion;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private int turnCount;
    private Team currentPlayerTurn;

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

        minionUnits[3,0] = SpawnSingleMinion(AllMinionSO[(int)team1[0]],Team.Player1);
        minionUnits[4,0] = SpawnSingleMinion(AllMinionSO[(int)team1[1]],Team.Player1);
        minionUnits[5,0] = SpawnSingleMinion(AllMinionSO[(int)team1[2]],Team.Player1);

        minionUnits[3,7] = SpawnSingleMinion(AllMinionSO[(int)team2[0]],Team.Player2);
        minionUnits[4,7] = SpawnSingleMinion(AllMinionSO[(int)team2[1]],Team.Player2);
        minionUnits[5,7] = SpawnSingleMinion(AllMinionSO[(int)team2[2]],Team.Player2);
        // int i, k;
        // foreach (MinionList minionId in team1){
        //     i = 3;
        //     k = 0;
        //     minionUnits[i,k] = SpawnSingleMinion(AllMinionSO[(int)minionId],Team.Player1);
        //     i++;
        //     if(i > 7){
        //         i = 0;
        //         k += 1;
        //     }
        // }

        // foreach (MinionList minionId in team2){
        //     i = 3;
        //     k = 7;
        //     minionUnits[i,k] = SpawnSingleMinion(AllMinionSO[(int)minionId],Team.Player2);
        //     i++;
        //     if(i > 7){
        //         i = 0;
        //         k -= 1;
        //     }
        // }
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
                minionUnits[x,y]?.MoveMinionUnit(Gameboard.Instance.GetTileCenter(x,y), true);
    } 
    private void SetUpTurn()
    {
        turnCount = 0;
        currentPlayerTurn = Team.Player1;
    }
    //Turn Logic
    private void RunTurnLogic(){
        Vector2Int currentHover = Gameboard.Instance.GetCurrentHover();
        if(selectedMinion){
            if(Input.GetMouseButtonDown(0)){
                if(currentHover == -Vector2Int.one)
                    DeselectMinion();
                else
                    if(minionUnits[currentHover.x, currentHover.y] == null)
                        if(IsValidMove(currentHover)){
                            MoveSelectedMinionTo(currentHover);
                            DeselectMinion();
                            FinishTurn();
                        }
                    else
                        if(minionUnits[currentHover.x, currentHover.y].Team == currentPlayerTurn)
                            SwitchSelectedMinion(currentHover);
  
            }
            if(Input.GetKeyDown(KeyCode.Q))
                MakeAttack1();
            if(Input.GetKeyDown(KeyCode.W))
                MakeAttack2();
        }
        else if(CanMinionBeSelected(currentHover))
            SelectMinion(currentHover);        
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
        selectedMinion = null;
        Gameboard.Instance.ChangeTilesLayers(availableMoves,"Tile");
        availableMoves.Clear();
    }
    private void SelectMinion(Vector2Int tileIndex){
        selectedMinion = minionUnits[tileIndex.x,tileIndex.y];
        availableMoves = selectedMinion.minion.GetAvailableMoves();
        Gameboard.Instance.ChangeTilesLayers(availableMoves,"Highlight");
        Debug.Log($"{selectedMinion.name} Selected");
    }
    private void SwitchSelectedMinion(Vector2Int tileIndex){
        DeselectMinion();
        SelectMinion(tileIndex);
    }    
    private void MoveSelectedMinionTo(Vector2Int newPosition){
        Vector2Int currentPosition = LookupMinionIndex(selectedMinion);
        if(currentPosition == newPosition)
            return;

        minionUnits[currentPosition.x, currentPosition.y] = null;
        minionUnits[newPosition.x, newPosition.y] = selectedMinion;
        selectedMinion.MoveMinionUnit(Gameboard.Instance.GetTileCenter(newPosition.x,newPosition.y));
    }
    private Vector2Int LookupMinionIndex(MinionUnit minion){
        for (int x = 0; x < Gameboard.TILE_COUNT_X; x++)
            for (int y = 0; y < Gameboard.TILE_COUNT_Y; y++)
                if(minionUnits[x,y] == minion)
                    return new Vector2Int(x,y);

        throw new System.Exception("LookupMinionIndex_NotFound");
        // return -Vector2Int.one;
    }
    private void HighlightAvailableMoves(){
        availableMoves = selectedMinion.minion.GetAvailableMoves();
        Gameboard.Instance.HightlightTiles(availableMoves);
    }
    
    private void MakeAttack2() {
        throw new NotImplementedException();
    }
    private void MakeAttack1(){
        throw new NotImplementedException();
    }

        private void FinishTurn()
    {
        turnCount++;
        if(currentPlayerTurn == Team.Player1)
            currentPlayerTurn = Team.Player2;
        else
            currentPlayerTurn = Team.Player1;

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
}
