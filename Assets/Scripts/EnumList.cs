using System;
using System.Diagnostics;

public enum Type
{
    None,
    Fire,
    Water,
    Earth,
    Light
}

public class TypeChart{
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
}

public enum Range
{
    None,
    CloseCombat,
    Distance
}

public enum Nature
{
    None,
    Common,
    Magic
}

public enum Status
{
    None,
    Burn,
    Paralysis,
    Sleep
}

public enum Team
{
    None,
    Player1,
    Player2
}

public enum MinionList
{
    None,
    Boy,
    Dwarf,
    Girl,
    Goblin,
    Mermaid,
    Paladin
}

public enum FaintedOptions
{
    None,
    MinionFainted,
    TrainerFainted,
    Invalid
}
