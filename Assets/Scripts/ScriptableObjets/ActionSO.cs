using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action", menuName = "ScriptableObject/Action", order = 2)]
public class ActionSO : ScriptableObject {
    [SerializeField] private new string name;
    [SerializeField] private string description;
    [SerializeField] private Type type;
    [SerializeField] private Range range;
    [SerializeField] private Nature nature;
    [SerializeField] private float power;
    [SerializeField] private float accuracy;
    [SerializeField] private float magicCost;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Animator animator;

    public string Name { get { return name; } }
    public string Description { get { return description; } }
    public Type Type { get { return type; } }
    public Range Range { get { return range; } }
    public Nature Nature { get { return nature; } }
    public float Power { get { return power; } }
    public float Accuracy { get { return accuracy; } }
    public float MagicCost { get { return magicCost; } }
    public Sprite Sprite { get { return sprite; } }
    public Animator Animator { get { return animator; } }
}



