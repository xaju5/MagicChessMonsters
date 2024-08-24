using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public ActionSO ActionInfo { get; private set; }
    public float MagicCost { get; private set; }

    public Action(ActionSO actionInfo){
        ActionInfo = actionInfo;
        MagicCost = actionInfo.MagicCost;
    }

    public List<Vector2Int> GetAvailableAttackTiles(ref MinionUnit[,] minionUnits, Vector2Int currentMinionIndex, int tile_count_x, int tile_count_y, Team enemyTeam){
        //TODO:Mas tipos de ataques
        int range = (int)ActionInfo.Range;
        List<Vector2Int> availableAttacks = MathUtils.GetAreaTiles(range, currentMinionIndex, tile_count_x, tile_count_y);
        for (int i = availableAttacks.Count - 1; i >= 0; i--)
            if (minionUnits[availableAttacks[i].x, availableAttacks[i].y]?.Team != enemyTeam)
                availableAttacks.RemoveAt(i);
        return availableAttacks;
    }
    
}
