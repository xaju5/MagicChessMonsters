using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Minion
{
    public MinionSO MinionInfo { get; private set; }

    public float health { get; private set; }
    public float magic { get; private set; }

    public Action action1 { get; private set; }
    public Action action2 { get; private set; }

    public Minion(MinionSO minionInfo){
        MinionInfo = minionInfo;
        health = minionInfo.HealthBase;
        magic = minionInfo.MagicBase;

        action1 = GetAction(minionInfo.LearnableActions, 0);
        action2 = GetAction(minionInfo.LearnableActions, 1);
    }
    private Action GetAction(List<ActionSO> actions, int index) {
        return (index < actions.Count && actions[index] != null) ? new Action(actions[index]) : null;
    }
    public float MaxHealth(){
        return MinionInfo.HealthBase;
    }
    
    public float MaxMagic(){
        return MinionInfo.MagicBase;
    }

    public List<Vector2Int> GetAvailableMoves(ref MinionUnit[,] minionUnits, Vector2Int currentMinionIndex, int tile_count_x, int tile_count_y){
        List<Vector2Int> availableMoves = null;
        int moveRange = MinionInfo.MovementRangeBase; 
        switch (MinionInfo.MovementType)
        {
            case SelectableTiles.Area:
                availableMoves = MathUtils.GetAreaTiles(moveRange, currentMinionIndex, tile_count_x, tile_count_y);
                break;

            case SelectableTiles.Star:
                availableMoves = MathUtils.GetStarTiles(moveRange, currentMinionIndex, tile_count_x, tile_count_y);
                break;

            case SelectableTiles.None:
                availableMoves = null;
                break;
        }
        for (int i = availableMoves.Count - 1; i >= 0 ; i--)
            if (minionUnits[availableMoves[i].x, availableMoves[i].y] != null)
                availableMoves.RemoveAt(i);
        return availableMoves;
    }
    
    private DamageDetails CalculateDamage(Action attackerAction, MinionSO attacker){
        float level = 1;
        float base_damage = 2 * level;
        float typeEffectiveness = MathUtils.GetEffectiviness(attackerAction.ActionInfo.Type, MinionInfo.Type);
        float diference = attackerAction.ActionInfo.Power * (attacker.Strength / MinionInfo.Defense);
        float critical = UnityEngine.Random.value <= 0.01 ? 2 : 1;
        float total_damage = (base_damage + diference) * critical * typeEffectiveness;
        
        return new DamageDetails(critical == 2, typeEffectiveness, total_damage);
    }

    public DamageDetails TakeDamage(Action attackerAction, MinionSO attacker){
        DamageDetails damageDetails = CalculateDamage(attackerAction, attacker);
        health -= damageDetails.total_damage;

        if(health <= 0){
            health = 0;
            damageDetails.isFainted = true;
            return damageDetails;
        }

        return damageDetails;
    }

    public bool Heal(float amount){
        health += amount;
        if(health >= MaxHealth()){
            health = MaxHealth();
            return true;
        }
        return false;
    }
    public bool ConsumeMagic(float amount){
        magic -= amount;
        if(magic <= 0){
            magic = 0;
            return true;
        }
        return false;
    }
    public bool RestoreMagic(float amount){
        magic += amount;
        if(magic >= MaxMagic()){
            magic = MaxMagic();
            return true;
        }
        return false;
    }
}

public class DamageDetails {
    public bool isFainted {get; set;}
    public bool isCritical {get; set;}
    public float typeEffectivines {get; set;}
    public float total_damage {get; set;}
    public FaintedOptions faintedOptions {get; set;}

    public DamageDetails(bool isCrytical, float typeEffectivines, float total_damage){
        this.isCritical = isCrytical;
        this.typeEffectivines = typeEffectivines;
        this.total_damage = total_damage;
        isFainted = false;
        faintedOptions = FaintedOptions.None;
    }
}