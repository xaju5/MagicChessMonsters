using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Minion", menuName = "ScriptableObject/Minion", order = 1)]
public class MinionSO : ScriptableObject {
    [SerializeField] private MinionList minionId;
    [SerializeField] private string description;
    [SerializeField] private Type type;
    [SerializeField] private float healthBase;
    [SerializeField] private float magicBase;
    [SerializeField] private float strength;
    [SerializeField] private float defense;
    [SerializeField] private float magicStrenght;
    [SerializeField] private float magicDefense;
    [SerializeField] private int movementRangeBase;
    [SerializeField] private SelectableTiles movementType;
    [SerializeField] private List<ActionSO> learnableActions;
    [SerializeField] private Sprite sprite;
    [SerializeField] private AnimatorOverrideController animator;

public MinionList MinionId { get { return minionId; } }
public string Description { get { return description; } }
public Type Type { get { return type; } }
public float HealthBase { get { return healthBase; } }
public float MagicBase { get { return magicBase; } }
public float Strength { get { return strength; } }
public float Defense { get { return defense; } }
public float MagicPower { get { return magicStrenght; } }
public float MagicResistance { get { return magicDefense; } }
public int MovementRangeBase { get { return movementRangeBase; } }
public SelectableTiles MovementType { get { return movementType; } }
public List<ActionSO> LearnableActions { get { return learnableActions; } }
public Sprite Sprite { get { return sprite; } }
public AnimatorOverrideController Animator { get { return animator; } }

}