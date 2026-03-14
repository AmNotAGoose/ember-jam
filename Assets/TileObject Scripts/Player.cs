using UnityEngine;

public class Player : TileObject
{
    Level level;
    int lastLayer = 0;

    public TileObject heldObject;

    void Start()
    {
        level = FindFirstObjectByType<Level>();

        type = "player";
        properties.Add(TileObjectProperies.Pushable);

        level.player = this;    
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) level.TryMovePlayer(0, 1);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) level.TryMovePlayer(0, -1);
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) level.TryMovePlayer(-1, 0);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) level.TryMovePlayer(1, 0);
        else if (Input.GetKeyDown(KeyCode.Space)) level.TryPlaceObject();
    }

    public void HoldObject(TileObject objectToHold)
    {
        heldObject = objectToHold;
        objectToHold.OnPicked();
    }

    public void DropObject()
    {
        heldObject.OnDropped(tile);
        heldObject = null;
    }

    public override void OnAffectedTickFinished()
    {
        base.OnAffectedTickFinished();
        level.UpdateLayers();
    }
}
