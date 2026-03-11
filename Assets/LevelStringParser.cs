using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct ParsedGrid
{
    public int width;
    public int height;
    public int layers;
    public List<ParsedTileObject> objects;
}

public struct ParsedTileObject
{
    public string type;
    public int x;
    public int y;
    public int layer;
    public List<string> options;
}

public class LevelStringParser
{
    /* 
        width|height|layer|=|type,x,y,layer,option1,option2,...
    */
    public static ParsedGrid GetParsedLevel(string levelString)
    {
        ParsedGrid parsedGrid = new ParsedGrid();

        string[] splitLevel = levelString.Split("|=|");

        string[] frontmatter = splitLevel[0].Split("|");
        parsedGrid.width = int.Parse(frontmatter[0]);
        parsedGrid.height = int.Parse(frontmatter[1]);
        parsedGrid.layers = int.Parse(frontmatter[2]);

        List<ParsedTileObject> gridObjects = new();

        if (!string.IsNullOrWhiteSpace(splitLevel[1]))
        {
            string[] body = splitLevel[1].Split("|");
            for (int i = 0; i < body.Length; i++)
            {
                string[] curOptions = body[i].Split(",");
                ParsedTileObject curObj = new();

                curObj.type = curOptions[0];
                curObj.x = int.Parse(curOptions[1]);
                curObj.y = int.Parse(curOptions[2]);
                curObj.layer = int.Parse(curOptions[3]);
                curObj.options = curOptions.Length >= 5 ? curOptions[4..].ToList() : new List<string>();

                gridObjects.Add(curObj);
            }
        }

        parsedGrid.objects = gridObjects;
        return parsedGrid;
    }
}
