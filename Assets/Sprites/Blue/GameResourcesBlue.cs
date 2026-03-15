using UnityEngine;

[CreateAssetMenu(fileName = "GameResources", menuName = "Scriptable Objects/GameResources")]
public class GameResources : ScriptableObject
{
    [Header("Floor")]
    public Sprite floor;

    [Header("outerWall Side Edges")]
    public Sprite outerWallLeft;
    public Sprite outerWallRight;

    [Header("outerWall Front")]
    public Sprite outerWallFront;
    public Sprite outerWallFrontCornerLeft;
    public Sprite outerWallFrontCornerRight;

    [Header("outerWall Top Edge")]
    public Sprite outerWallTop;
    public Sprite outerWallTopCornerLeft;
    public Sprite outerWallTopCornerRight;

    public Sprite outerWallTopBottom;
    public Sprite outerWallLeftRight;
    public Sprite outerWallLeftTopRight;
    public Sprite outerWallLeftBottomRight;
    public Sprite outerWallLeftTopBottomRight;



    [Header("innerWall Side Edges")]
    public Sprite innerWallLeft;
    public Sprite innerWallRight;

    [Header("innerWall Front")]
    public Sprite innerWallFront;
    public Sprite innerWallFrontCornerLeft;
    public Sprite innerWallFrontCornerRight;

    [Header("innerWall Top Edge")]
    public Sprite innerWallTop;
    public Sprite innerWallTopCornerLeft;
    public Sprite innerWallTopCornerRight;

    public Sprite innerWallTopBottom;
    public Sprite innerWallLeftRight;
    public Sprite innerWallLeftTopRight;
    public Sprite innerWallLeftBottomRight;
}
