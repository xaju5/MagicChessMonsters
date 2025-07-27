using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public ActionSO ActionInfo { get; private set; }
    public float MagicCost { get; private set; }

    public Action(ActionSO actionInfo)
    {
        ActionInfo = actionInfo;
        MagicCost = actionInfo.MagicCost;
    }

    public List<Vector2Int> GetAvailableAttackTiles(ref MinionUnit[,] minionUnits, Vector2Int currentMinionIndex, Team enemyTeam)
    {
        List<Vector2Int> availableAttacks = null;
        int range = (int)ActionInfo.Range;
        switch (ActionInfo.RangeType)
        {
            case SelectableTiles.Area:
                availableAttacks = MathUtils.GetAreaTiles(range, currentMinionIndex);
                break;

            case SelectableTiles.Star:
                availableAttacks = MathUtils.GetStarTiles(range, currentMinionIndex);
                break;

            case SelectableTiles.None:
                availableAttacks = null;
                break;
        }

        for (int i = availableAttacks.Count - 1; i >= 0; i--)
            if (minionUnits[availableAttacks[i].x, availableAttacks[i].y]?.Team != enemyTeam || minionUnits[availableAttacks[i].x, availableAttacks[i].y]?.minion.IsFainted() == true)
                availableAttacks.RemoveAt(i);
        return availableAttacks;
    }
    
    
    public List<Vector2Int> GetAvailableSummonTiles(ref MinionUnit[,] minionUnits, Vector2Int currentMinionIndex)
    {
        int range = (int)ActionInfo.Range;
        List<Vector2Int> availableSummons = MathUtils.GetAreaTiles(range, currentMinionIndex);
        for (int i = availableSummons.Count - 1; i >= 0 ; i--)
            if (minionUnits[availableSummons[i].x, availableSummons[i].y] != null)
                availableSummons.RemoveAt(i);
        return availableSummons;
    }
}
