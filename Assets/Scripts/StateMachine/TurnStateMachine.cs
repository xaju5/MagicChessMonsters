using System.Collections;
using System.Collections.Generic;
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
    private List<Vector2Int> availableMoves = new List<Vector2Int>();

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

    //Logic Funtions
    public MinionUnit GetMinionUnits(int x, int y){
        return minionUnits[x,y];
    }

    public void DeselectMinion(){
        selectedMinion = null;
    //     DeselectAction();
    //     UIManager.Instance.RemoveSelectedMinionUI();
        Gameboard.Instance.ChangeTilesLayers(availableMoves,"Tile");
        availableMoves.Clear();
    }

    public void SelectMinion(Vector2Int tileIndex){
        selectedMinion = minionUnits[tileIndex.x,tileIndex.y];
        // UIManager.Instance.SetupSelectedMinionUI(selectedMinion.minion);
        
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
}
