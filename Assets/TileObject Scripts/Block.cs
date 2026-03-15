using UnityEngine;

public class Block : TileObject
{
    Level level;

    void Start()
    {
        level = FindFirstObjectByType<Level>();

        type = "block";
        properties.Add(TileObjectProperies.Pushable);
        sprite.sprite = level.resources.box;    
    }
}
