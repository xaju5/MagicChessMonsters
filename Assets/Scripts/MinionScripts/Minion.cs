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

    public List<Vector2Int> GetAvailableMoves(){
        List<Vector2Int> availableMoves = new List<Vector2Int>();
        availableMoves.Add(new Vector2Int(3, 3));
        availableMoves.Add(new Vector2Int(3, 4));
        availableMoves.Add(new Vector2Int(4, 3));
        availableMoves.Add(new Vector2Int(4, 4));
        return availableMoves;
    }
}
