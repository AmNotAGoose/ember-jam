using UnityEngine;

public class Bomb : TileObject
{
    Level level;

    public int detonationTime;
    public int curTime;
    public bool detonated;
    public bool exploded;

    void Start()
    {
        level = FindFirstObjectByType<Level>();
        level.onPlayerMoveSubscribers.Add(this);

        type = "bomb";
        detonationTime = int.Parse(options[0]);
        curTime = detonationTime;
        properties.Add(TileObjectProperies.Holdable);
    }

    public override void OnPicked()
    {
        base.OnPicked();
        print("picked");
        tile.PopObject(this);
        transform.SetParent(level.player.transform);
    }

    public override void OnDropped(Tile newTile)
    {
        base.OnDropped(newTile);
        newTile.AddObject(this);
        detonated = true;
        properties.Remove(TileObjectProperies.Holdable);
    }

    public override void OnPlayerMove()
    {
        base.OnPlayerMove();
        if (detonated && !exploded)
        {
            curTime -= 1;
            if (curTime <= 0)
            {
                Explode();
            }
        }
    }

    public virtual void Explode()
    {
        Hole newHole = Instantiate(level.prefabDict["hole"], Vector3.zero, Quaternion.identity).GetComponent<Hole>();
        tile.PopObject(this);
        tile.AddObject(newHole);
        level.TickPlayerTile();
        exploded = true;
        Destroy(gameObject);
    }
}
