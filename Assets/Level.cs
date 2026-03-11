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
        Initialize("5|5|3|=|");
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
            curTileObject.transform.SetParent(curTile.transform);
        }
    }
}
