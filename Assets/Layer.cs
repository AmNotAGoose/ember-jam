using UnityEngine;

public class Layer : MonoBehaviour
{
    public int startingLayer;
    public int curLayer;

    public void SetStartingLayer(int layer)
    {
        startingLayer = layer;
        curLayer = layer;
    }


}
