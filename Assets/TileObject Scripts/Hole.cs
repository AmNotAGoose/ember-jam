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
    }
    public override void OnTileObjectAdded()
    {
        transform.localScale = Vector3.one;
        base.OnTileObjectAdded();
        UpdateTexture();
    }

    public override void OnAffectedTickFinished()
    {
        base.OnAffectedTickFinished();
        if (level == null) level = FindFirstObjectByType<Level>();

        if (tile.tileObjects.Count > 1) // hole is treated as a tile object
        {
            level.TryDrop(this);
        }
        UpdateTexture();
    }

    public override void OnPlayerMove()
    {
        base.OnPlayerMove();

        sprite.sprite = level.GetHoleTexture(this); 
    } 

    public void UpdateTexture()
    {
        if (level == null) level = FindFirstObjectByType<Level>();
        sprite.sprite = level.GetHoleTexture(this); // and to think a level editor was originally planned
    }
}
