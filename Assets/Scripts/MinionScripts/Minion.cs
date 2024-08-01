using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Minion
{
    public MinionSO MinionInfo { get; private set; }

    private float health;
    private float magic;

    public Minion(MinionSO minionInfo){
        MinionInfo = minionInfo;
        health = minionInfo.HealthBase;
        magic = minionInfo.MagicBase;
    }

    public float MaxHealth(){
        return MinionInfo.HealthBase;
    }
    
    public float MaxMagic(){
        return MinionInfo.MagicBase;
    }

    public List<Vector2Int> GetAvailableMoves(ref MinionUnit[,] minionUnits, Vector2Int currentMinionIndex, int tile_count_x, int tile_count_y){
        //TODO:Mas tipos de movimientos. Optimizar!
        List<Vector2Int> availableMoves = new List<Vector2Int>();

        int moveRange = MinionInfo.MovementRangeBase; 
        //Movimiento en estrella:
        int[,] directions = new int[,] { {-1, 0}, {0, -1}, {0, 1}, {1, 0}, {-1, -1}, {1, -1}, {-1, 1}, {1, 1} };
        for (int i = 1; i <= moveRange; i++)
        {
            for (int d = 0; d < directions.GetLength(0); d++) {
                int newX = currentMinionIndex.x + directions[d, 0] * i;
                int newY = currentMinionIndex.y + directions[d, 1] * i;

                if (newX >= 0 && newX < tile_count_x && newY >= 0 && newY < tile_count_y) {
                    if (minionUnits[newX, newY] == null) {
                        availableMoves.Add(new Vector2Int(newX, newY));
                    }
                }
            }
        }
        return availableMoves;
    }
}