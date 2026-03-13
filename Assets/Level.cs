using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int width;
    public int height;
    public int numLayers;

    int roomScaleFactor = 1;

    public GameObject gridParent;
    public GameObject layerParentPrefab;
    public GameObject tilePrefab;

    public Tile[,,] tiles;
    public List<Layer> layers;

    [System.Serializable]
    public struct TileObjectPrefabEntry
    {
        public string type;
        public GameObject prefab;
    }
    public List<TileObjectPrefabEntry> tileObjectPrefabs;

    private void Start()
    {
        Initialize("5|5|3|=|player,3,3,0|wall,4,3,0|block,2,3,0");
    }

    public void Initialize(string levelString)
    {
        ParsedGrid parsedGrid = LevelStringParser.GetParsedLevel(levelString);

        width = parsedGrid.width;
        height = parsedGrid.height;
        numLayers = parsedGrid.layers;

        if (width == 0 || height == 0 || numLayers == 0) return;
        tiles = new Tile[width, height, numLayers];

        for (int k = 0; k < numLayers; k++) // layers
        {
            GameObject curLayerObj = Instantiate(layerParentPrefab, Vector3.zero, Quaternion.identity);
            curLayerObj.transform.SetParent(gridParent.transform);
            curLayerObj.transform.localPosition = Vector3.zero;

            Layer curLayer = curLayerObj.GetComponent<Layer>();
            curLayer.SetStartingLayer(k);
            layers.Add(curLayer);

            for (int i = 0; i < width; i++) // width
            {
                for (int j = 0; j < height; j++) // height
                {
                    GameObject curTileObj = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity);
                    curTileObj.transform.SetParent(curLayerObj.transform);
                    curTileObj.transform.localPosition = new Vector3(
                        i - (width - 1) / 2f,
                        j - (height - 1) / 2f,
                        0
                    );
                    Tile curTile = curTileObj.GetComponent<Tile>();
                    curTile.x = i;
                    curTile.y = j;
                    curTile.k = k;
                    tiles[i, j, k] = curTile;
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
        if (x < 0 || x >= width || y < 0 || y >= height || k < 0 || k >= numLayers) return null;
        return tiles[x, y, k];
    }

    public void TryMoveOnLayer(TileObject objectToMove, int x, int y)
    {
        Tile objTile = objectToMove.tile;

        int curX = objTile.x; 
        int curY = objTile.y;
        int curK = objTile.k;

        List<Tile> affectedTiles = new();
        bool canMove = false;

        int temp = 0;
        while (temp < 10)
        {
            Tile curTile = GetTile(curX, curY, curK);

            if (curTile == null) break;
            if (curTile.IsStopping()) break;

            affectedTiles.Add(curTile); // theres something here that can be pushed or moved through

            if (curTile.IsPushable())
            {
                curX += x;
                curY += y;
                continue; // if its pushable, check the next tile
            }

            // if its not stopper, not pushable, and in bounds, then we can move for sure
            canMove = true;
            break;
        }

        if (!canMove) return;

        List<TileObject> poppedObjects = new();
        foreach (Tile tile in affectedTiles)
        {
            List<TileObject> nextPoppedObjects = tile.PopObjects(TileObjectProperies.Pushable);
            tile.AddObjects(poppedObjects);
            poppedObjects = nextPoppedObjects;
        } // the last one should not have any pushable as defined earlier


    }

    public void TryDrop(TileObject objectToDrop)
    {

    }
}
