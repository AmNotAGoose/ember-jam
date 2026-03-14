using UnityEngine;

public class Player : TileObject
{
    Level level;
    int lastLayer = 0;

    void Start()
    {
        level = FindFirstObjectByType<Level>();

        type = "player";
        properties.Add(TileObjectProperies.Pushable);

        level.player = this;    
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) level.TryMoveOnLayer(this, 0, 1);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) level.TryMoveOnLayer(this, 0, -1);
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) level.TryMoveOnLayer(this, -1, 0);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) level.TryMoveOnLayer(this, 1, 0);
    }

    public override void OnAffectedTickFinished()
    {
        base.OnAffectedTickFinished();
        level.UpdateLayers();
    }
}
