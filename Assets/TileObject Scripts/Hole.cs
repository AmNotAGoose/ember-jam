using UnityEngine;

public class Hole : TileObject
{
    Level level;
    public bool isWinHole = false;

    void Start()
    {
        level = FindFirstObjectByType<Level>();

        level.onPlayerMoveSubscribers.Add(this);

        type = "hole";
        if (options.Count > 0 )
        {
            isWinHole = options[0] == "true";
        }
        sprite.sprite = level.GetHoleTexture(this);
    }
    public override void OnTileObjectAdded()
    {
        transform.localScale = Vector3.one;
        base.OnTileObjectAdded();
    }

    public override void OnAffectedTickFinished()
    {
        base.OnAffectedTickFinished();
        if (level == null) level = FindFirstObjectByType<Level>();

        if (tile.tileObjects.Count > 1) // hole is treated as a tile object
        {
            level.TryDrop(this);
        }
    }

    public override void OnPlayerMove()
    {
        base.OnPlayerMove();

        sprite.sprite = level.GetHoleTexture(this); 
    }
}
