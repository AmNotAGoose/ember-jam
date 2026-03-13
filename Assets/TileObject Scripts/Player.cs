using UnityEngine;

public class Player : TileObject
{
    Level level;

    void Start()
    {
        level = FindFirstObjectByType<Level>();

        type = "player";
        properties.Add(TileObjectProperies.Stopper);
    }

    private void Update()
    {
        
    }
}
