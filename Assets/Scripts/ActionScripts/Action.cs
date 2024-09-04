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
        List<Vector2Int> availableAttacks = null;
        int range = (int)ActionInfo.Range + 5;
        switch (ActionInfo.RangeType)
        {
            case SelectableTiles.Area:
                availableAttacks = MathUtils.GetAreaTiles(range, currentMinionIndex, tile_count_x, tile_count_y);
                break;

            case SelectableTiles.Star:
                availableAttacks = MathUtils.GetStarTiles(range, currentMinionIndex, tile_count_x, tile_count_y);
                break;

            case SelectableTiles.None:
                availableAttacks = null;
                break;
        }

        for (int i = availableAttacks.Count - 1; i >= 0; i--)
            if (minionUnits[availableAttacks[i].x, availableAttacks[i].y]?.Team != enemyTeam)
                availableAttacks.RemoveAt(i);
        return availableAttacks;
    }
    
}
