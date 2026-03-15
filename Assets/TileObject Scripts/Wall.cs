using UnityEngine;

public class Wall : TileObject
{
    void Start()
    {
        type = "wall";
        properties.Add(TileObjectProperies.Stopper);
    }

    public void SetInnerSprite(Sprite spr)
    {
        sprite.sprite = spr;
    }
}
