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
    public int curLayerIdx = 0; // further down is higher

    int roomScaleFactor = 1;

    public GameObject gridParent;
    public GameObject layerParentPrefab;
    public GameObject tilePrefab;

    public Tile[,,] tiles;
    public List<Layer> layers;

    public Player player;

    [System.Serializable]
    public struct TileObjectPrefabEntry
    {
        public string type;
        public GameObject prefab;
    }
    public List<TileObjectPrefabEntry> tileObjectPrefabs;

    private void Start()
    {
        Initialize("5|5|3|=|player,3,3,0|wall,4,3,0|block,2,3,0|hole,1,1,0|hole,1,2,1|hole,1,3,2");
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
            curLayer.startingLayer = k;
            curLayer.SetLayerActive(curLayerIdx);
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
                    curTile.layer = curLayer;
                    tiles[i, j, k] = curTile;
                }
            }
        }

        layers[^1].isLastLayer = true;

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

        while (true)
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

        foreach (Tile tile in affectedTiles)
        {
            tile.OnAffectedTickFinished();
        }
    }

    public void TryDrop(Tile objectTile)
    {
        int x = objectTile.x;
        int y = objectTile.y;
        int objectLayer = objectTile.k;

        int nextLayer = (objectLayer + 1) % numLayers;
        Tile targetTile = GetTile(x, y, nextLayer); 

        if (targetTile == null) return;
        if (targetTile.IsStopping() || targetTile.IsPushable()) return; 

        List<TileObject> popped = objectTile.PopObjects(TileObjectProperies.Pushable);
        targetTile.AddObjects(popped);

        targetTile.OnAffectedTickFinished();
    }

    public void UpdateLayers()
    {
        foreach (var layer in layers)
        {
            layer.SetLayerActive(player.tile.k);
        }
    }
}
