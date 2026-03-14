using System.Collections.Generic;
using UnityEngine;

public enum TileObjectProperies
{
    Pushable,
    Stopper,
    Holdable
}

public abstract class TileObject : MonoBehaviour
{
    public Tile tile;
    public int objectId;
    public string type;
    public List<TileObjectProperies> properties;
    public SpriteRenderer sprite;
    public List<string> options;

    public virtual void OnTileObjectRemoved() { }
    public virtual void OnTileObjectAdded()
    {
        transform.localPosition = Vector3.zero; //temp for now w/o animaiton
    }
    public virtual void OnAffectedTickFinished() { }
    public virtual void OnPlayerMove() { }
    public virtual void OnPicked() { }
    public virtual void OnDropped(Tile newTile) { }
}
