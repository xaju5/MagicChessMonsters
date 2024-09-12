using UnityEngine;


[CreateAssetMenu(fileName = "TileState", menuName = "ScriptableObject/TileState", order = 0)]
public class TileStateSO : ScriptableObject {
    [SerializeField] private new string name;
    [SerializeField] private string description;
    [SerializeField] private Type type;
    [SerializeField] private Status status;
    [SerializeField] private float durationInTurns;
    [SerializeField] private bool blockMinionPlacement;

    public string Name { get { return name;}}
    public string Description { get { return description;}}
    public Type Type { get { return type;}}
    public Status Status { get { return status;}}
    public float DurationInTurns { get { return durationInTurns; }}
    public bool BlockMinionPlacement { get { return blockMinionPlacement; }}
}

