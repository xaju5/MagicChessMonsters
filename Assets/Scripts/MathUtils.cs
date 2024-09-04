using System.Collections.Generic;
using UnityEngine;

public class MathUtils
{
    static float[,] typeChart =  {
            //Fi   wa   ear  lig    
   /*fire*/  {1f, 0.5f, 0.5f, 2f},
   /*water*/ {2f, 1f, 2f, 1f},
   /*earth*/ {2f, 0.5f, 1f, 0f},
   /*light*/ {0.5f, 1f, 2f, 1f}
};
    public static float GetEffectiviness(Type attack, Type defense)
    {
        if (attack == Type.None || defense == Type.None)
            return 1f;
        int row = (int)attack - 1;
        int col = (int)defense - 1;
        return typeChart[row, col];
    }

    public static List<Vector2Int> GetStarTiles(int range, Vector2Int centerPosition, int max_x, int max_y){
        List<Vector2Int> availableTiles = new List<Vector2Int>();
        int[,] directions = new int[,] { {-1, 0}, {0, -1}, {0, 1}, {1, 0}, {-1, -1}, {1, -1}, {-1, 1}, {1, 1} };
        for (int i = 1; i <= range; i++)
            for (int d = 0; d < directions.GetLength(0); d++) 
            {
                Vector2Int tilePosition = new Vector2Int(centerPosition.x + directions[d, 0] * i, centerPosition.y + directions[d, 1] * i);
                if (CheckMustSkipTile(tilePosition, centerPosition, max_x, max_y)) continue;
                availableTiles.Add(tilePosition);
            }
        return availableTiles;
    }
    public static List<Vector2Int> GetAreaTiles(int range, Vector2Int centerPosition, int max_x, int max_y)
    {
        List<Vector2Int> availableTiles = new List<Vector2Int>();
        int areaSize = 1 + 2 * range;
        for (int x = 0; x < areaSize; x++)
            for (int y = 0; y < areaSize; y++)
            {
                Vector2Int tilePosition = new Vector2Int(x - range, y - range) + centerPosition;
                if (CheckMustSkipTile(tilePosition, centerPosition, max_x, max_y)) continue;
                availableTiles.Add(tilePosition);
            }
        return availableTiles;
    }
    private static bool CheckMustSkipTile(Vector2 tilePosition, Vector2Int centerPosition, int max_x, int max_y)
    {
        if (tilePosition == centerPosition) return true; //CurrentPosition
        if (tilePosition.x < 0 || tilePosition.y < 0) return true; //OutOfBound
        if (tilePosition.x > max_x - 1 || tilePosition.y > max_y - 1) return true; //OutOfBound

        return false;
    }

    public static float GetVectorAngle(Vector2 vector)
    {
        if (vector.x < 0)
            return 360 - (Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg * -1);
        else
            return Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
    }


}
