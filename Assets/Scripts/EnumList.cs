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
