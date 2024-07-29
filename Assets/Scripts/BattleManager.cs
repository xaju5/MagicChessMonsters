using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private MinionList[] team1, team2;
    [SerializeField] private MinionSO[] AllMinionSO;

    private MinionUnit[,] minionUnits;
    private MinionUnit selectedMinion;

    public static BattleManager Instance;

    private void Awake()
    {
        SetUpSingleton();
    }
    void Start()
    {
        SpawnAllMinions();
        PositionAllMinions();
    }
    void Update() {
        //TODO: Select between move or attack
        Vector2Int currentHover = Gameboard.Instance.GetCurrentHover();
        if(Input.GetMouseButtonDown(0) && currentHover != -Vector2Int.one){
            if(selectedMinion == null)
                SelectMinion(currentHover.x, currentHover.y);
            else
                MoveSelectedMinionTo(currentHover.x, currentHover.y);
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

    private void MoveSelectedMinionTo(int x, int y){
        if (minionUnits[x,y] != null)
            return;
        
        Vector2Int currentPosition = LookupMinionIndex(selectedMinion);
        if(currentPosition == new Vector2Int(x,y))
            return;

        minionUnits[currentPosition.x, currentPosition.y] = null;
        minionUnits[x,y] = selectedMinion;
        selectedMinion.MoveMinionUnit(Gameboard.Instance.GetTileCenter(x,y));
        selectedMinion = null;
    }
    
    private void SelectMinion(int x, int y)
    {
        if (minionUnits[x, y] == null)
            return;

        //TODO: Turn management
        if (true)
        {
            selectedMinion = minionUnits[x,y];
            Debug.Log($"{selectedMinion.name} Selected");
        }
    }

    private Vector2Int LookupMinionIndex(MinionUnit minion){
        for (int x = 0; x < Gameboard.TILE_COUNT_X; x++)
            for (int y = 0; y < Gameboard.TILE_COUNT_Y; y++)
                if(minionUnits[x,y] == minion)
                    return new Vector2Int(x,y);

        throw new System.Exception("LookupMinionIndex_NotFound");
        // return -Vector2Int.one;
    }
}
