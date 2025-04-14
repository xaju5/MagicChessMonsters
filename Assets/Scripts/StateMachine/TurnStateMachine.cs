using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnStateMachine : StateMachine
{
    [HideInInspector] public SetUpState setUpState;
    [HideInInspector] public SelectionState selectionState;
    [HideInInspector] public MoveState moveState;

    [SerializeField] private GameObject minionPrefab, actionPrefab;
    [SerializeField] private MinionList[] team1, team2;
    [SerializeField] private MinionSO[] AllMinionSO;

    private MinionUnit selectedMinion;
    private MinionUnit[,] minionUnits;
    
    private Team currentPlayerTurn;
    private List<ActionUnit> pendingAnimations = new List<ActionUnit>();

    private void Awake()
    {
        setUpState = new SetUpState(this);
        selectionState = new SelectionState(this);
        moveState = new MoveState(this);
    }

    protected override BaseState GetInitialState()
    {
        return setUpState;
    }

    //General Logic Funtions
    public MinionUnit GetMinionUnits(int x, int y){
        if (minionUnits == null) return null;
        return minionUnits[x,y];
    }
    public ref MinionUnit[,] GetMinionUnitsArray(){
        return ref minionUnits;
    }

    public void DeselectMinion(){
        selectedMinion = null;
    //     DeselectAction();
    }

    public void SelectMinion(Vector2Int tileIndex){
        selectedMinion = minionUnits[tileIndex.x,tileIndex.y];
    }

    public MinionUnit GetSelectedMinion(){
        return selectedMinion;
    }
    public MinionUnit SpawnSingleMinion(MinionSO minionInfo, Team team, Vector2Int initialIndex){
        GameObject minionGO = Instantiate(minionPrefab, transform);
        minionGO.name = minionInfo.MinionId.ToString();
        minionGO.GetComponent<SpriteRenderer>().sortingOrder = Gameboard.Instance.GetTilemapRenderer().sortingOrder + 2;

        MinionUnit minionUnit = minionGO.GetComponent<MinionUnit>();
        minionUnit.SetUpData(minionInfo, team, initialIndex);

        minionUnits[initialIndex.x,initialIndex.y] = minionUnit;

        return minionUnit;
    }

    public void RemoveMinionFromBattleground(MinionUnit minion, bool force = false)
    {
        if(force) Destroy(minion.gameObject);
        minionUnits[minion.MinionIndex.x,minion.MinionIndex.y] = null;
    }

    public MinionSO GetTeamMinionSO(int index, Team playerTeam){
        if(playerTeam.Equals(Team.Player1))
            return AllMinionSO[(int)team1[index]];
        else
            return AllMinionSO[(int)team2[index]];
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

    public Team GetCurrentPlayerTurn(){
        return currentPlayerTurn;
    }
}
