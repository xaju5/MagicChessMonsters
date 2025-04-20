using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class TurnStateMachine : StateMachine
{
    [HideInInspector] public SetUpState setUpState;
    [HideInInspector] public SelectionState selectionState;
    [HideInInspector] public MoveState moveState;
    [HideInInspector] public AttackState attackState;
    [HideInInspector] public EndTurnState endTurnState;
    [HideInInspector] public GameoverState gameoverState;

    [SerializeField] private GameObject minionPrefab, actionPrefab;
    [SerializeField] private MinionList[] team1_enum, team2_enum;
    [SerializeField] private MinionSO[] AllMinionSO;

    private MinionUnit selectedMinion;
    private MinionUnit[,] minionUnits;
    private List<MinionUnit> minionUnitList = new List<MinionUnit>();
    
    private Team currentPlayerTurn;
    private List<ActionUnit> pendingAnimations = new List<ActionUnit>();

    private void Awake()
    {
        setUpState = new SetUpState(this);
        selectionState = new SelectionState(this);
        moveState = new MoveState(this);
        attackState = new AttackState(this);
        endTurnState = new EndTurnState(this);
        gameoverState = new GameoverState(this);
    }

    protected override BaseState GetInitialState()
    {
        return setUpState;
    }

    //Minion Unit Array Management
    public MinionUnit GetMinionUnit(Vector2Int index){
        if (minionUnits == null) return null;
        return minionUnits[index.x,index.y];
    }
    public ref MinionUnit[,] GetMinionUnitsArray(){
        return ref minionUnits;
    }
    public void UpdateMinionPositionInArray(MinionUnit minion, Vector2Int newPosition)
    {
        minionUnits[selectedMinion.MinionIndex.x, selectedMinion.MinionIndex.y] = null;
        minionUnits[newPosition.x, newPosition.y] = minion;
    }

    public MinionUnit SpawnSingleMinion(MinionSO minionInfo, Team team, Vector2Int initialIndex){
        GameObject minionGO = Instantiate(minionPrefab, transform);
        minionGO.name = minionInfo.MinionId.ToString();
        minionGO.GetComponent<SpriteRenderer>().sortingOrder = Gameboard.Instance.GetTilemapRenderer().sortingOrder + 2;

        MinionUnit minionUnit = minionGO.GetComponent<MinionUnit>();
        minionUnit.SetUpData(minionInfo, team, initialIndex);

        minionUnits[initialIndex.x,initialIndex.y] = minionUnit;
        minionUnitList.Add(minionUnit);

        return minionUnit;
    }

    public void RemoveMinionFromBattleground(MinionUnit minion, bool force = false)
    {
        if(force) Destroy(minion.gameObject);
        minionUnits[minion.MinionIndex.x,minion.MinionIndex.y] = null;
        minionUnitList.Remove(minion);
    }

    public List<MinionUnit> GetMinionUnitList(Team team = Team.None){
        if (team == Team.None)
            return minionUnitList;
        return minionUnitList.Where(minionUnit => minionUnit.Team == team).ToList();      
    }

    //General Logic Funtions
    public MinionSO GetTeamMinionSO(int index, Team playerTeam){
        if(playerTeam.Equals(Team.Player1))
            return AllMinionSO[(int)team1_enum[index]];
        else
            return AllMinionSO[(int)team2_enum[index]];
    }

    public void ClearLogicVariables()
    {
        currentPlayerTurn = Team.Player1;
        minionUnits = new MinionUnit[Gameboard.TILE_COUNT_X,Gameboard.TILE_COUNT_Y];
        // isGameover = false;
        // isGamePaused = false;
        pendingAnimations.Clear();
    }

    public List<Vector2Int> GetTeamMinionPositions(Team team){
        List<Vector2Int> minionPositions = new List<Vector2Int>();
        for (int x = 0; x < Gameboard.TILE_COUNT_X; x++)
            for (int y = 0; y < Gameboard.TILE_COUNT_Y; y++)
                if(minionUnits[x,y]?.Team == team)
                    minionPositions.Add(new Vector2Int(x,y));
        return minionPositions;
    }

    //Player & Oponent turn
    public Team GetCurrentPlayerTurn(){
        return currentPlayerTurn;
    }
    public void SwitchPlayerTurn(){
        currentPlayerTurn = GetEnemyTeam();
    }

    private Team GetEnemyTeam(){
        if(currentPlayerTurn == Team.Player1)
            return Team.Player2;
        else
            return Team.Player1;
    }

    //Selection
    public void DeselectMinion(){
        selectedMinion = null;
    //     DeselectAction();
    }

    public MinionUnit SelectMinion(Vector2Int tileIndex){
        selectedMinion = minionUnits[tileIndex.x,tileIndex.y];
        return selectedMinion;
    }

    public MinionUnit GetSelectedMinion(){
        return selectedMinion;
    }

}
