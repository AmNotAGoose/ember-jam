using UnityEngine;

public class Goal : TileObject
{
    Level level;
    public bool satisfied = false;

    void Start()
    {
        level = GetComponent<Level>();
        level.goals.Add(this);
    }

    public override void OnAffectedTickFinished()
    {
        base.OnAffectedTickFinished();
        satisfied = tile.tileObjects.Count > 1;
    }
}
