using UnityEngine;

[CreateAssetMenu(fileName = "GameResources", menuName = "Scriptable Objects/GameResources")]
public class GameResources : ScriptableObject
{
    [Header("Base Textures")]
    public Sprite floor;
    public Sprite wall;

    [Header("1 by x or x by 1 textures")]
    public Sprite wallTopBottom;
    public Sprite wallLeftRight;
    public Sprite wallTopLeftRight;
    public Sprite wallBottomLeftRight;

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
}
