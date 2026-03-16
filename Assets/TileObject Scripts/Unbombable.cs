using UnityEngine;

public class Unbombable : TileObject
{
    Level level;
    void Start()
    {
        level = FindFirstObjectByType<Level>();
        type = "unbombable";
        properties.Add(TileObjectProperies.Unbombable);
        sprite.sprite = level.resources.unbombable;
    }
}
