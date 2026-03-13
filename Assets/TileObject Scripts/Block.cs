using UnityEngine;

public class Block : TileObject
{
    void Start()
    {
        type = "block";
        properties.Add(TileObjectProperies.Pushable);
    }
}
