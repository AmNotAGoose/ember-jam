using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Tile : MonoBehaviour
{
    List<TileObject> tileObjects;

    public TileObject PopObject(int objectId)
    {
        TileObject obj = tileObjects.Find(o => o.objectId == objectId);
        if (obj == null) return null;
        tileObjects.Remove(obj);
        return obj;
    }
    public TileObject PopObject(string type)
    {
        TileObject obj = tileObjects.Find(o => o.type == type);
        if (obj == null) return null;
        tileObjects.Remove(obj);
        return obj;
    }

    public void AddObject(TileObject objectToAdd)
    {
        tileObjects.Append(objectToAdd);
    }
}
