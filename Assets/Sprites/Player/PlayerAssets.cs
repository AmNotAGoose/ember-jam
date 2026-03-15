using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAssets : MonoBehaviour
{
    public GameObject up;
    public GameObject down;
    public GameObject left;
    public GameObject right;
    public GameObject upIdle;
    public GameObject downIdle;
    public GameObject leftIdle;
    public GameObject rightIdle;
    public GameObject activeObject;
    private void Start()
    {
        activeObject = downIdle;
        activeObject.SetActive(true);
    }
    public void SetActiveAnimation(int x, int y)
    {
        GameObject next;
        if (y > 0) next = up;
        else if (y < 0) next = down;
        else if (x > 0) next = right;
        else next = left;
        SwitchActive(next);
    }
    public void SetIdleAnimation()
    {
        GameObject next;
        if (activeObject == up || activeObject == upIdle) next = upIdle;
        else if (activeObject == down || activeObject == downIdle) next = downIdle;
        else if (activeObject == right || activeObject == rightIdle) next = rightIdle;
        else next = leftIdle;
        SwitchActive(next);
    }
    private void SwitchActive(GameObject next)
    {
        if (next == activeObject) return;
        activeObject.SetActive(false);
        activeObject = next;
        activeObject.SetActive(true);
    }
}