using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;


public class Tile : MonoBehaviour
{
    public int x;
    public int y;
    public int k;
    public List<TileObject> tileObjects;

    public List<TileObject> PopObjects(TileObjectProperies property)
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
        return tileObjects.Count > 0 && tileObjects.All(o => o.properties.Contains(TileObjectProperies.Pushable));
    }

    public bool IsStopping()
    {
        return tileObjects.Count > 0 && tileObjects.All(o => o.properties.Contains(TileObjectProperies.Stopper));
    }
}
