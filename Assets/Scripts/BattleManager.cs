using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private MinionList[] team1, team2;
    [SerializeField] private MinionSO[] AllMinionSO;

    private Minion[,] minionUnits;

    void Start()
    {
        SpawnAllMinions();
    }

    private void SpawnAllMinions(){

        foreach (MinionList minionId in team1)
            SpawnSingleMinion(AllMinionSO[(int)minionId],Team.Player1);

        foreach (MinionList minionId in team2)
            SpawnSingleMinion(AllMinionSO[(int)minionId],Team.Player2);

    }
    private MinionUnit SpawnSingleMinion(MinionSO minionInfo, Team team){
        GameObject minionGO = Instantiate(minionPrefab, transform);
        minionGO.name = minionInfo.MinionId.ToString();
        minionGO.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<TilemapRenderer>().sortingOrder + 2;
        MinionUnit minionUnit = minionGO.GetComponent<MinionUnit>();
        minionUnit.SetUpData(minionInfo, team);

        return minionUnit;
    }
}
