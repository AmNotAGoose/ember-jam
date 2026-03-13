using System.Collections.Generic;
using UnityEngine;

public enum TileObjectProperies
{
    Pushable,
    Stopper
}

public abstract class TileObject : MonoBehaviour
{
    public Tile tile;
    public int objectId;
    public string type;
    public List<TileObjectProperies> properties;
    public SpriteRenderer sprite;

    public virtual void OnTileObjectRemoved() { }
    public virtual void OnTileObjectAdded() { }
}
