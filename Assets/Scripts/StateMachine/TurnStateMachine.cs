using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class TurnStateMachine : StateMachine
{
    [HideInInspector] public SetUpState setUpState;
    [HideInInspector] public SelectionState selectionState;
    [HideInInspector] public ChooseState chooseState;
    [HideInInspector] public MoveState moveState;
    [HideInInspector] public AttackState attackState;
    [HideInInspector] public EndTurnState endTurnState;
    [HideInInspector] public GameoverState gameoverState;
    [HideInInspector] public SummonState summonState;

    [SerializeField] private GameObject minionPrefab, actionPrefab;
    [SerializeField] private MinionList[] team1_enum, team2_enum;
    [SerializeField] private MinionSO[] AllMinionSO;

    private MinionUnit selectedMinion;
    private Action selectedAction;
    private Vector2Int targetPosition;
    private MinionUnit[,] minionUnits;
    private List<MinionUnit> minionUnitList = new List<MinionUnit>();
    private Team currentPlayerTurn;
    private AnimationTimer animationTimer;
    private Team winner;
    private MinionUnit minionToSummon;

    private void Awake()
    {
        setUpState = new SetUpState(this);
        selectionState = new SelectionState(this);
        chooseState = new ChooseState(this);
        moveState = new MoveState(this);
        attackState = new AttackState(this);
        endTurnState = new EndTurnState(this);
        gameoverState = new GameoverState(this);
        summonState = new SummonState(this);
        UIManager.Instance.resetEvent.AddListener(ResetGame);
    }

    protected override BaseState GetInitialState()
    {
        return setUpState;
    }

    //Minion Unit Array Management
    public MinionUnit GetMinionUnit(Vector2Int index)
    {
        if (minionUnits == null) return null;
        return minionUnits[index.x, index.y];
    }
    public ref MinionUnit[,] GetMinionUnitsArray()
    {
        return ref minionUnits;
    }
    public void UpdateMinionPositionInArray(MinionUnit minion, Vector2Int newPosition)
    {
        minionUnits[selectedMinion.MinionIndex.x, selectedMinion.MinionIndex.y] = null;
        minionUnits[newPosition.x, newPosition.y] = minion;
    }

    public MinionUnit SpawnSingleMinion(MinionSO minionInfo, Team team)
    {
        GameObject minionGO = Instantiate(minionPrefab, transform);
        minionGO.name = minionInfo.MinionId.ToString();
        minionGO.GetComponent<SpriteRenderer>().sortingOrder = Gameboard.Instance.GetTilemapRenderer().sortingOrder + 2;
        minionGO.SetActive(false);

        MinionUnit minionUnit = minionGO.GetComponent<MinionUnit>();
        minionUnit.SetUpData(minionInfo, team, -Vector2Int.one);
        minionUnitList.Add(minionUnit);

        return minionUnit;
    }

    public void SpawnAllMinions()
    {
        for (int i = 0; i < team1_enum.Length; i++)
        {
            SpawnSingleMinion(GetTeamMinionSO(i, Team.Player1), Team.Player1);
        }
        for (int i = 0; i < team2_enum.Length; i++)
        {
            SpawnSingleMinion(GetTeamMinionSO(i, Team.Player2), Team.Player2);
        }
    }

    private MinionSO GetTeamMinionSO(int index, Team playerTeam)
    {
        if (playerTeam.Equals(Team.Player1))
            return AllMinionSO[(int)team1_enum[index]];
        else
            return AllMinionSO[(int)team2_enum[index]];
    }

    public void SummonMinion(MinionUnit minionUnit, Vector2Int initialIndex)
    {
        minionUnit.gameObject.SetActive(true);
        minionUnit.minion.SummonMinion();
        minionUnit.MoveMinionUnit(initialIndex, true);
        minionUnits[initialIndex.x, initialIndex.y] = minionUnit;
    }

    public void RemoveMinionFromBattleground(MinionUnit minion, bool force = false)
    {
        minionUnits[minion.MinionIndex.x, minion.MinionIndex.y] = null;
        minionUnitList.Remove(minion);
        if (force) Destroy(minion.gameObject);
    }

    public List<MinionUnit> GetMinionUnitList(Team team = Team.None)
    {
        if (team == Team.None)
            return minionUnitList;
        return minionUnitList.Where(minionUnit => minionUnit.Team == team).ToList();
    }

    public List<MinionUnit> GetTrainers()
    {
        return minionUnitList.Where(minionUnit => minionUnit.IsTrainer).ToList();
    }

    public void UpdateAllMinionUnitGraphics()
    {
        foreach (MinionUnit minionUnit in minionUnitList)
            minionUnit.UpdateMinionUnitGraphics();
    }

    //General Logic Funtions
    public void ClearLogicVariables()
    {
        currentPlayerTurn = Team.Player1;
        minionUnits = new MinionUnit[Gameboard.TILE_COUNT_X, Gameboard.TILE_COUNT_Y];
        winner = Team.None;
    }

    public List<Vector2Int> GetTeamAliveMinionPositions(Team team, bool alive = true)
    {
        List<Vector2Int> minionPositions = new List<Vector2Int>();
        for (int x = 0; x < Gameboard.TILE_COUNT_X; x++)
            for (int y = 0; y < Gameboard.TILE_COUNT_Y; y++)
                if (minionUnits[x, y]?.Team == team && minionUnits[x, y]?.minion.IsFainted() == false)
                    minionPositions.Add(new Vector2Int(x, y));
        return minionPositions;
    }

    //Player & Oponent turn
    public Team GetCurrentPlayerTurn()
    {
        return currentPlayerTurn;
    }
    public void SwitchPlayerTurn()
    {
        currentPlayerTurn = GetEnemyTeam();
    }

    public Team GetEnemyTeam()
    {
        if (currentPlayerTurn == Team.Player1)
            return Team.Player2;
        else
            return Team.Player1;
    }

    //Minion Selection
    public void DeselectMinion()
    {
        selectedMinion = null;
    }

    public MinionUnit SelectMinion(Vector2Int tileIndex)
    {
        selectedMinion = minionUnits[tileIndex.x, tileIndex.y];
        return selectedMinion;
    }

    public MinionUnit GetSelectedMinion()
    {
        return selectedMinion;
    }

    //Action Logic
    public void SetTargetPosition(Vector2Int target)
    {
        targetPosition = target;
    }
    public Vector2Int GetTargetPosition()
    {
        return targetPosition;
    }
    public void DeselectTargetPosition()
    {
        targetPosition = -Vector2Int.one;
    }


    public void SelectAction(Action action)
    {
        selectedAction = action;
    }

    public void DeselectAction()
    {
        selectedAction = null;
    }

    public Action GetSelectedAction()
    {
        return selectedAction;
    }

    public void SetAnimationTimer(AnimationTimer animationTimer)
    {
        this.animationTimer = animationTimer;
    }

    public AnimationTimer GetAnimationTimer()
    {
        return animationTimer;
    }

    public ActionUnit SpawnAction(Action action, Vector3 targetPosition)
    {
        float movementAngle = MathUtils.GetVectorAngle(selectedMinion.transform.position - targetPosition); //TODO: Rotar ataque hacia enemigo
        GameObject actionGO = Instantiate(actionPrefab, selectedMinion.transform.position, new Quaternion());
        actionGO.name = action.ActionInfo.Name;
        actionGO.GetComponent<SpriteRenderer>().sortingOrder = Gameboard.Instance.GetTilemapRenderer().sortingOrder + 3;
        ActionUnit actionUnit = actionGO.GetComponent<ActionUnit>();
        actionUnit.SetUpData(action, targetPosition);
        return actionUnit;
    }

    public void StartActionAnimationTimer(List<ActionUnit> pendingAnimations)
    {
        SetAnimationTimer(AnimationTimer.Waiting);
        StartCoroutine(WaitForAnimations(pendingAnimations));
    }

    private IEnumerator WaitForAnimations(List<ActionUnit> pendingAnimations)
    {
        while (pendingAnimations.Count > 0)
        {
            for (int i = pendingAnimations.Count - 1; i >= 0; i--)
            {
                if (pendingAnimations[i].HasAnimationFinished())
                {
                    Destroy(pendingAnimations[i].gameObject);
                    pendingAnimations.RemoveAt(i);
                }
            }
            yield return null;
        }
        SetAnimationTimer(AnimationTimer.Finished);
    }

    // Summon

    public void SetMinionToSummon(MinionUnit minion)
    {
        minionToSummon = minion;
    }

    public MinionUnit GetMinionToSummon()
    {
        return minionToSummon;
    }

    public void DeselectMinionToSummon()
    {
        minionToSummon = null;
    }

    //Gameover
    public void SetWinner(Team team)
    {
        winner = team;
    }

    public Team GetWinner()
    {
        return winner;
    }

    private void ResetGame()
    {
        ChangeState(setUpState);
        if(isGamePaused) TogglePause();
    }

}
