using UnityEngine;

[CreateAssetMenu(fileName = "GameResources", menuName = "Scriptable Objects/GameResources")]
public abstract class GameResources : ScriptableObject
{
    public GameObject playerAssets;
    public GameObject music;
    public GameObject fakeTile;

    public Sprite filledHole;
    public Sprite emptyHole;
    public Sprite winFilledHole;
    public Sprite winEmptyHole;

    public Sprite unbombable;

    public Sprite box;
    public Sprite goalUnsatisfied;
    public Sprite goalSatisfied;

    [Header("Base Textures")]
    public Sprite floor;
    public Sprite wall;

    [Header("1 by x or x by 1 textures")]
    public Sprite wallTopBottom;
    public Sprite wallLeftRight;
    public Sprite wallTopLeftRight;
    public Sprite wallBottomLeftRight;
    public Sprite wallTopBottomLeft;
    public Sprite wallTopBottomRight;

    [Header("Edge textures")]
    public Sprite wallTop;
    public Sprite wallBottom;
    public Sprite wallRight;
    public Sprite wallLeft;

    [Header("Inner corners ")]
    public Sprite innerCornerBottomRight;
    public Sprite innerCornerBottomLeft;
    public Sprite innerCornerTopRight;
    public Sprite innerCornerTopLeft;

    [Header("Outer corners")]
    public Sprite outerCornerBottomRight;
    public Sprite outerCornerBottomLeft;
    public Sprite outerCornerTopRight;
    public Sprite outerCornerTopLeft;

    //[Header("double corners and stuff")]
    public Sprite wallIsolated;
    //public Sprite outerCornerTopLeft_InnerBottomRight;
    //public Sprite outerCornerTopRight_InnerBottomLeft;
    //public Sprite outerCornerBottomLeft_InnerTopRight;
    //public Sprite outerCornerBottomRight_InnerTopLeft;
    //public Sprite outerCornersTopLeftBottomRight;
    //public Sprite outerCornersTopRightBottomLeft;



    [Header("Special corners")] // the mentioned parts are the inner section
    public Sprite spCornerBottomRight;
    public Sprite spCornerBottomLeft;
    public Sprite spCornerTopRight;
    public Sprite spCornerTopLeft;
}
