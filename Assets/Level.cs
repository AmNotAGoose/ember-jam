using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int width;
    public int height;
    public int layers;

    int roomScaleFactor = 1;

    public GameObject gridParent;
    public GameObject layerParentPrefab;
    public GameObject tilePrefab;

    public Tile[,,] tiles;

    [System.Serializable]
    public struct TileObjectPrefabEntry
    {
        public string type;
        public GameObject prefab;
    }
    public List<TileObjectPrefabEntry> tileObjectPrefabs;

    private void Start()
    {
        Initialize("5|5|3|=|player,3,3,0");
    }

    public void Initialize(string levelString)
    {
        ParsedGrid parsedGrid = LevelStringParser.GetParsedLevel(levelString);

        width = parsedGrid.width;
        height = parsedGrid.height;
        layers = parsedGrid.layers;

        if (width == 0 || height == 0 || layers == 0) return;
        tiles = new Tile[width, height, layers];

        for (int k = 0; k < layers; k++) // layers
        {
            GameObject curLayer = Instantiate(layerParentPrefab, Vector3.zero, Quaternion.identity);
            curLayer.transform.SetParent(gridParent.transform);
            curLayer.transform.localPosition = Vector3.zero;
            curLayer.GetComponent<Layer>().SetStartingLayer(k);

            for (int i = 0; i < width; i++) // width
            {
                for (int j = 0; j < height; j++) // height
                {
                    GameObject curTile = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity);
                    curTile.transform.SetParent(curLayer.transform);
                    curTile.transform.localPosition = new Vector3(
                        i - (width - 1) / 2f,
                        j - (height - 1) / 2f,
                        0
                    );
                    tiles[i, j, k] = curTile.GetComponent<Tile>();
                }
            }
        }

        Dictionary<string, GameObject> prefabDict = tileObjectPrefabs.ToDictionary(e => e.type, e => e.prefab);

        foreach (ParsedTileObject parsedTileObject in parsedGrid.objects)
        {
            GameObject curTileObject = Instantiate(prefabDict[parsedTileObject.type], Vector3.zero, Quaternion.identity);
            Tile curTile = tiles[parsedTileObject.x, parsedTileObject.y, parsedTileObject.layer];
            //curTileObject.transform.SetParent(curTile.transform);
            curTile.AddObject(curTileObject.GetComponent<TileObject>());
        }
    }

    public Tile GetTile(int x, int y, int k)
    {
        if (x < 0 || x >= width || y < 0 || y >= height || k < 0 || k >= layers) return null;
        return tiles[x, y, k];
    }

    public void TryMoveOnLayer(TileObject objectToMove, int x, int y)
    {
        Tile objTile = objectToMove.tile;

        int curX = objTile.x; 
        int curY = objTile.y;
        int curK = objTile.k;

        bool canMove = false;

        while (true)    
        {
            Tile curTile = GetTile(curX, curY, curK);
            if (curTile == null) break;
            if (curTile.IsStopping()) break;
            if (curTile.IsPushable())
            {
                curX += x;
                curY += y;
                continue; // if its pushable, check the next tile
            }

            // if its not stopper, not pushable, and in bounds, then we can move for sure
            canMove = true;
        }
    }

    public void TryDrop(TileObject objectToDrop)
    {

    }
}
