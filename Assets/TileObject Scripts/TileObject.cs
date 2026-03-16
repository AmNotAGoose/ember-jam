using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileObjectProperies
{
    Pushable,
    Stopper,
    Holdable,
    Unbombable
}

public abstract class TileObject : MonoBehaviour
{
    public Tile tile;
    public int objectId;
    public string type;
    public List<TileObjectProperies> properties;
    public SpriteRenderer sprite;
    public List<string> options;
    public bool isMoving;

    float duration = 0.05f;

    public virtual IEnumerator MoveToLocalOrigin()
    {
        isMoving = true;    
        float t = 0f;
        Vector3 startingPosition = transform.localPosition;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            transform.localPosition = Vector3.right * Mathf.Lerp(startingPosition.x, 0f, progress) + Vector3.up * Mathf.Lerp(startingPosition.y, 0f, progress); 
            yield return null;
        }
        transform.localPosition = Vector3.zero;
        isMoving = false;
    } 

    public virtual void OnTileObjectRemoved() { }
    public virtual void OnTileObjectAdded()
    {
        if (properties != null && properties.Contains(TileObjectProperies.Pushable))
        {
            StopAllCoroutines();
            StartCoroutine(MoveToLocalOrigin());
        }
        else
        {
            transform.localPosition = Vector3.zero; //temp for now w/o animaiton
        }
    }
    public virtual void OnAffectedTickFinished() { }
    public virtual void OnPlayerMove() { }
    public virtual void OnPicked() { }
    public virtual void OnDropped(Tile newTile) { }
    public virtual void OnChangedLayers() { } // not implement
    public virtual void OnPickedFinished() { }
}
