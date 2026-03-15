using System;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameResources resources;

    public int width;
    public int height;
    public int numLayers;
    public int curLayerIdx = 0; // further down is higher

    float roomScaleFactor = 1;

    public GameObject gridParent;
    public GameObject layerParentPrefab;
    public GameObject tilePrefab;

    public Tile[,,] tiles;
    public List<Wall> walls;
    public List<Layer> layers;

    public Dictionary<string, GameObject> prefabDict;

    // set by various tileobject scripts
    public Player player;
    public List<Goal> goals;
    public List<TileObject> onPlayerMoveSubscribers;

    public string theme;

    [System.Serializable]
    public struct TileObjectPrefabEntry
    {
        public string type;
        public GameObject prefab;
    }
    public List<TileObjectPrefabEntry> tileObjectPrefabs;

    private void Start()
    {
        Initialize(
"8|6|3|=|wall,0,0,0|block,3,3,0|wall,1,0,0|wall,2,0,0|wall,3,0,0|wall,4,0,0|wall,5,0,0|wall,6,0,0|wall,7,0,0|wall,0,1,0|wall,7,1,0|wall,0,2,0|wall,7,2,0|wall,0,3,0|wall,7,3,0|wall,0,4,0|wall,7,4,0|wall,0,5,0|wall,1,5,0|wall,2,5,0|wall,3,5,0|wall,4,5,0|wall,5,5,0|wall,6,5,0|wall,7,5,0|wall,0,0,1|wall,1,0,1|wall,2,0,1|wall,3,0,1|wall,4,0,1|wall,5,0,1|wall,6,0,1|wall,7,0,1|wall,0,1,1|wall,7,1,1|wall,0,2,1|wall,7,2,1|wall,0,3,1|wall,7,3,1|wall,0,4,1|wall,7,4,1|wall,0,5,1|wall,1,5,1|wall,2,5,1|wall,3,5,1|wall,4,5,1|wall,5,5,1|wall,6,5,1|wall,7,5,1|wall,0,0,2|wall,1,0,2|wall,2,0,2|wall,3,0,2|wall,4,0,2|wall,5,0,2|wall,6,0,2|wall,7,0,2|wall,0,1,2|wall,7,1,2|wall,0,2,2|wall,7,2,2|wall,0,3,2|wall,7,3,2|wall,0,4,2|wall,7,4,2|wall,0,5,2|wall,1,5,2|wall,2,5,2|wall,3,5,2|wall,4,5,2|wall,5,5,2|wall,6,5,2|wall,7,5,2|player,3,2,0|bomb,2,2,0,4|hole,4,2,0|hole,5,3,0|goal,5,2,1|hole,3,3,1|goal,4,4,2"
);
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
            curLayer.totalLayers = numLayers;
            curLayer.startingLayer = k;
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
                    curTile.SetInnerSprite(resources.floor);
                }
            }
        }

        layers[^1].isLastLayer = true;

        prefabDict = tileObjectPrefabs.ToDictionary(e => e.type, e => e.prefab);

        foreach (ParsedTileObject parsedTileObject in parsedGrid.objects)
        {
            GameObject curTileObject = Instantiate(prefabDict[parsedTileObject.type], Vector3.zero, Quaternion.identity);
            TileObject uhhhhhhhhhhhhhh = curTileObject.GetComponent<TileObject>();
            uhhhhhhhhhhhhhh.options = parsedTileObject.options;
            Tile curTile = tiles[parsedTileObject.x, parsedTileObject.y, parsedTileObject.layer];
            curTile.AddObject(uhhhhhhhhhhhhhh);
        }

        foreach (Layer curLayer in layers)
        {
            curLayer.SetLayerVisible(false);
        }
        layers[0].SetLayerVisible(true);

        foreach (Wall wall in FindObjectsByType<Wall>(FindObjectsSortMode.None))
        {
            wall.SetInnerSprite(GetWallTexture(wall));
        }

        foreach (Hole hole in FindObjectsByType<Hole>(FindObjectsSortMode.None))
        {
            hole.sprite.sprite = GetHoleTexture(hole);
        }

        FitGridToCamera();
    }
    public void FitGridToCamera(float margin = 0.9f)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;

        float scaleX = (camWidth / width) * margin;
        float scaleY = (camHeight / height) * margin;

        roomScaleFactor = (int)Mathf.Min(scaleX, scaleY); 

        gridParent.transform.localScale = Vector3.one * Mathf.Min(scaleX, scaleY);
    }

    public Tile GetTile(int x, int y, int k)
    {
        if (x < 0 || x >= width || y < 0 || y >= height || k < 0 || k >= numLayers) return null;
        return tiles[x, y, k];
    }

    public void TryMovePlayer(int x, int y)
    {
        TryMoveOnLayer(player, x, y);

        foreach (TileObject playerMoveSubscriber in onPlayerMoveSubscribers)
        {
            playerMoveSubscriber.OnPlayerMove();
        }

        TryPickUpObject();
    }

    void TryPickUpObject()
    {
        if (player.heldObject != null) return;

        TileObject objectToHold = player.tile.tileObjects.Find(o => o.properties.Contains(TileObjectProperies.Holdable));
        if (objectToHold == null) return;
        
        player.HoldObject(objectToHold);
    }

    public void TryPlaceObject()
    {
        if (player.heldObject == null) return;
        player.DropObject();
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
            List<TileObject> nextPoppedObjects = tile.PopObjectsByProperty(TileObjectProperies.Pushable);
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

        List<TileObject> popped = objectTile.PopObjectsByProperty(TileObjectProperies.Pushable);
        targetTile.AddObjects(popped);

        targetTile.OnAffectedTickFinished();
    }

    public void TickPlayerTile()
    {
        player.tile.OnAffectedTickFinished();
    }

    public void UpdateLayers()
    {
        int newLayerIdx = player.tile.k;
        if (newLayerIdx == curLayerIdx) return;
        layers[curLayerIdx].TransitionLayers(false);
        layers[newLayerIdx].TransitionLayers(true);
        curLayerIdx = newLayerIdx;
    }

    public bool IsWinning()
    {
        return goals.All(goal => goal.satisfied);
    }

    public void Win()
    {

    }

    public Sprite GetHoleTexture(Hole hole)
    {
        int nextLayer = (hole.tile.k + 1) % numLayers;
        Tile nextTile = tiles[hole.tile.x, hole.tile.y, nextLayer];
        if (!nextTile.IsPushable() && !nextTile.IsStopping())
        {
            return resources.emptyHole;
        }
        return resources.filledHole;    
    }

    public Sprite GetWallTexture(Wall wall)
    {
        Tile tile = wall.tile;
        int x = tile.x, y = tile.y, k = tile.k;

        bool top = GetTile(x, y + 1, k)?.HasWall() != true;
        bool bottom = GetTile(x, y - 1, k)?.HasWall() != true;
        bool left = GetTile(x - 1, y, k)?.HasWall() != true;
        bool right = GetTile(x + 1, y, k)?.HasWall() != true;

        // 3 sides exposed
        if (top && bottom && left && !right) return resources.wallTopBottomLeft;
        if (top && bottom && right && !left) return resources.wallTopBottomRight;
        if (top && left && right && !bottom) return resources.wallTopLeftRight;
        if (bottom && left && right && !top) return resources.wallBottomLeftRight;

        // 2 opposite sides exposed
        if (top && bottom) return resources.wallTopBottom;
        if (left && right) return resources.wallLeftRight;

        // 2 adjacent sides exposed = outer corner
        if (top && right) return resources.outerCornerTopRight;
        if (top && left) return resources.outerCornerTopLeft;
        if (bottom && right) return resources.outerCornerBottomRight;
        if (bottom && left) return resources.outerCornerBottomLeft;

        // 1 side exposed
        if (top) return resources.wallTop;
        if (bottom) return resources.wallBottom;
        if (left) return resources.wallLeft;
        if (right) return resources.wallRight;

        // 0 cardinal floors (check diagonals for inner corners )
        bool diagTopRight = GetTile(x + 1, y + 1, k)?.HasWall() != true;
        bool diagTopLeft = GetTile(x - 1, y + 1, k)?.HasWall() != true;
        bool diagBottomRight = GetTile(x + 1, y - 1, k)?.HasWall() != true;
        bool diagBottomLeft = GetTile(x - 1, y - 1, k)?.HasWall() != true;

        if (diagTopRight) return resources.innerCornerTopRight;
        if (diagTopLeft) return resources.innerCornerTopLeft;
        if (diagBottomRight) return resources.innerCornerBottomRight;
        if (diagBottomLeft) return resources.innerCornerBottomLeft;

        return resources.wall;
    }
}
