using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine; 
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    public GameResources resources;

    public SoundManager soundManager;

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
     
    public string levelString;

    [System.Serializable]
    public struct TileObjectPrefabEntry
    {
        public string type;
        public GameObject prefab;
    }
    public List<TileObjectPrefabEntry> tileObjectPrefabs;

    public GameObject levelStartScreen;
    public GameObject infLoopScreen;
    public GameObject winScreen;

    public bool lastHoleWasWinning;

    public string nextLevel;

    private void Start()
    {
        StartCoroutine(ShowScreenThenInitialize());
    }

    public IEnumerator ShowScreenThenInitialize()
    {
        yield return new WaitForSeconds(0.5f);
        levelStartScreen.SetActive(false);
        Initialize(levelString);
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

            int border = 10;
            for (int i = -border; i < width + border; i++)
            {
                for (int j = -border; j < height + border; j++)
                {
                    if (i >= 0 && i < width && j >= 0 && j < height) continue;

                    GameObject fakeTileObj = Instantiate(resources.fakeTile, Vector3.zero, Quaternion.identity);
                    fakeTileObj.transform.SetParent(curLayerObj.transform);
                    fakeTileObj.transform.localPosition = new Vector3(
                        i - (width - 1) / 2f,
                        j - (height - 1) / 2f,
                        0
                    );
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

        foreach (Goal goal in FindObjectsByType<Goal>(FindObjectsSortMode.None))
        {
            goal.sprite.sprite = GetGoalTexture(goal);
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

    public IEnumerator ShowInfiniteLoopAndRestart()
    {
        foreach (AudioSource source in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            source.Stop();
        }
        soundManager.bombExplode.Play();
        infLoopScreen.SetActive(true);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

        soundManager.PlayFootstep();

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

    public void TryDrop(Hole hole)
    {
        Tile objectTile = hole.tile;
        int x = objectTile.x;
        int y = objectTile.y;
        int objectLayer = objectTile.k;

        if (hole.isWinHole && objectTile == player.tile && goals.All(goal => goal.satisfied))
        {
            Win();
            return;
        }

        int nextLayer = (objectLayer + 1) % numLayers;
        Tile targetTile = GetTile(x, y, nextLayer); 

        if (targetTile == null) return;
        if (targetTile.IsStopping() || targetTile.IsPushable()) return;
        if (WillInfiniteLoop(x, y)) {
            StartCoroutine(ShowInfiniteLoopAndRestart());
            return;
        }

        lastHoleWasWinning = hole.isWinHole;
        soundManager.layerFall.Play();

        List<TileObject> popped = objectTile.PopObjectsByProperty(TileObjectProperies.Pushable);
        targetTile.AddObjects(popped);
        layers[nextLayer].RefreshRenderers();
        targetTile.OnAffectedTickFinished();
    }

    public bool WillInfiniteLoop(int x, int y)
    {
        for (int k = 0; k < numLayers; k++)
        {
            Tile tile = GetTile(x, y, k);
            if (tile == null || !tile.HasHole()) return false;
        }
        return true;
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
        if (!lastHoleWasWinning)
        {
            lastHoleWasWinning = false; 
            return false;
        }
        lastHoleWasWinning = false;
        return goals.All(goal => goal.satisfied);
    }

    public void Win()
    {
        print("wewin");
        foreach (AudioSource source in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            source.Stop();
        }
        soundManager.winLevel.Play();
        StartCoroutine(PlayWinEffectsAndLoadNext());
    }

    public IEnumerator PlayWinEffectsAndLoadNext()
    {
        winScreen.SetActive(true);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(nextLevel);
    }

    public Sprite GetGoalTexture(Goal goal)
    {
        return goal.satisfied ? resources.goalSatisfied : resources.goalUnsatisfied;
    }

    public Sprite GetHoleTexture(Hole hole)
    {
        int nextLayer = (hole.tile.k + 1) % numLayers;
        Tile nextTile = tiles[hole.tile.x, hole.tile.y, nextLayer];
        if (!nextTile.IsPushable() && !nextTile .IsStopping())
        {
            if (hole.isWinHole)
            {
                return resources.winEmptyHole;
            }
            return resources.emptyHole;
        }
        if (hole.isWinHole)
        {
            return resources.winFilledHole;
        }
        return resources.filledHole;    
    }

    public Sprite GetWallTexture(Wall wall) // thank you mr. claude
    {
        Tile tile = wall.tile;
        int x = tile.x, y = tile.y, k = tile.k;

        bool top = GetTile(x, y + 1, k)?.HasWall() == false;
        bool bottom = GetTile(x, y - 1, k)?.HasWall() == false;
        bool left = GetTile(x - 1, y, k)?.HasWall() == false;
        bool right = GetTile(x + 1, y, k)?.HasWall() == false;

        bool diagTopRight = GetTile(x + 1, y + 1, k)?.HasWall() == false;
        bool diagTopLeft = GetTile(x - 1, y + 1, k)?.HasWall() == false;
        bool diagBottomRight = GetTile(x + 1, y - 1, k)?.HasWall() == false;
        bool diagBottomLeft = GetTile(x - 1, y - 1, k)?.HasWall() == false;

        if (top && bottom && left && right) return resources.wallIsolated;

        // 3 sides exposed
        if (top && bottom && left && !right) return resources.wallTopBottomLeft;
        if (top && bottom && right && !left) return resources.wallTopBottomRight;
        if (top && left && right && !bottom) return resources.wallTopLeftRight;
        if (bottom && left && right && !top) return resources.wallBottomLeftRight;

        // 2 opposite sides exposed
        if (top && bottom) return resources.wallTopBottom;
        if (left && right) return resources.wallLeftRight;

        // 2 adjacent sides exposed = outer corner (or special corner if opposite diagonal is also floor)
        if (top && right) return diagBottomLeft ? resources.spCornerBottomLeft : resources.outerCornerTopRight;
        if (top && left) return diagBottomRight ? resources.spCornerBottomRight : resources.outerCornerTopLeft;
        if (bottom && right) return diagTopLeft ? resources.spCornerTopLeft : resources.outerCornerBottomRight;
        if (bottom && left) return diagTopRight ? resources.spCornerTopRight : resources.outerCornerBottomLeft;

        // 1 side exposed
        if (top) return resources.wallTop;
        if (bottom) return resources.wallBottom;
        if (left) return resources.wallLeft;
        if (right) return resources.wallRight;

        // 0 cardinal floors — check diagonals for inner corners
        if (diagTopRight) return resources.innerCornerTopRight;
        if (diagTopLeft) return resources.innerCornerTopLeft;
        if (diagBottomRight) return resources.innerCornerBottomRight;
        if (diagBottomLeft) return resources.innerCornerBottomLeft;

        return resources.wall;
    }
}
