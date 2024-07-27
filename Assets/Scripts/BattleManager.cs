using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private MinionList[] team1, team2;
    [SerializeField] private MinionSO[] AllMinionSO;

    private MinionUnit[,] minionUnits;

    void Start()
    {
        SpawnAllMinions();
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
        minionGO.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<TilemapRenderer>().sortingOrder + 2;

        MinionUnit minionUnit = minionGO.GetComponent<MinionUnit>();
        minionUnit.SetUpData(minionInfo, team);

        return minionUnit;
    }

    private void PositionAllMinions(){

    } 

    private void PositionSingleMinion(int x, int y, bool force = false){

    }
}
