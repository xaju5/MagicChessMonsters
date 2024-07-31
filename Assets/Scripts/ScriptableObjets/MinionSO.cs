using System.Collections;
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
    [SerializeField] private List<ActionSO> learnableActions;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Animator animator;

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
public Sprite Sprite { get { return sprite; } }
public Animator Animator { get { return animator; } }

}