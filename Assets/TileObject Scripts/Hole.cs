using UnityEngine;

public class Hole : TileObject
{
    Level level;

    void Start()
    {
        level = FindFirstObjectByType<Level>();

        type = "hole";
    }

    public override void OnAffectedTickFinished()
    {
        base.OnAffectedTickFinished();

        if (tile.tileObjects.Count > 1) // hole is treated as a tile object
        {
            level.TryDrop(tile);
        }
    }
}
