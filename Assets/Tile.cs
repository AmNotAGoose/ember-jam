using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Tile : MonoBehaviour
{
    public int x;
    public int y;
    public int k;
    public List<TileObject> tileObjects;
    public Layer layer;
    public SpriteRenderer tileSprite;

    public TileObject PopObject(TileObject objectToRemove)
    { 
        if (tileObjects.Remove(objectToRemove))
        {
            objectToRemove.OnTileObjectRemoved();
            return objectToRemove;
        }
        return null;
    }

    public List<TileObject> PopObjectsByProperty(TileObjectProperies property)
    {
        List<TileObject> objs = tileObjects.FindAll(o => o.properties.Contains(property));

        foreach (TileObject obj in objs)
        {
            tileObjects.Remove(obj); // it is assumed that every pop will follow an add, or the object shoudl be destroyed  
            obj.OnTileObjectRemoved();
        }

        return objs;
    }

    public void AddObject(TileObject objectToAdd)
    {
        tileObjects.Add(objectToAdd);
        objectToAdd.tile = this;
        objectToAdd.transform.SetParent(transform);
        objectToAdd.OnTileObjectAdded();
    }

    public void AddObjects(List<TileObject> objectsToAdd)
    {
        foreach (TileObject obj in objectsToAdd)
        {
            AddObject(obj);
        }
    }

    public bool IsPushable()
    {
        return tileObjects.Any(o => o.properties.Contains(TileObjectProperies.Pushable))
            && !tileObjects.Any(o => o.properties.Contains(TileObjectProperies.Stopper));
    }

    public bool IsStopping()
    {
        return tileObjects.Count > 0 && tileObjects.Any(o => o.properties.Contains(TileObjectProperies.Stopper));
    }
    public void OnAffectedTickFinished()
    {
        foreach (TileObject tileObject in tileObjects.ToList()) // this is a patch fix dont need to make a copy of objects
        {
            tileObject.OnAffectedTickFinished();
        }
    } 
    public bool HasWall()
    {
        return tileObjects.Exists(o => o is Wall);
    }

    public void SetInnerSprite(Sprite spr)
    {
        tileSprite.sprite = spr;
    }
}
