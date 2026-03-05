using UnityEngine;

public class Level : MonoBehaviour
{
    public int width;
    public int height;

    public GameObject gridParent;
    public GameObject tilePrefab;

    public Tile[,] grid;

    private void Start()
    {
        Initialize("");
    }

    public void Initialize(string levelString)
    {
        ParsedGrid parsedGrid = LevelStringParser.GetParsedLevel(levelString);

        width = parsedGrid.width;
        height = parsedGrid.height;

        if (width == 0 || height == 0) return;
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject curTile = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity);
            }
        }

        for (int i = 0; i < parsedGrid.objects.Count; i++)
        {

        }
    }
}
