using System.Collections;
using UnityEngine;

public class Layer : MonoBehaviour
{
    public int startingLayer;
    public GameObject layerObject;

    public bool isLastLayer; 

    public Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetLayerActive(int curLayer)
    {
        if (isLastLayer)
        {
            if (curLayer == startingLayer) Appear();
            else if (curLayer == 0) Disappear();
            return;
        }

        if (curLayer == startingLayer) Appear();
        else if (curLayer == startingLayer + 1) Disappear();
    }

    void Disappear() // DisappearToAbove
    {
        animator.Play("DisappearToAbove");
        //gameObject.SetActive(false);
    }

    void Appear() // AppearingFromBelow
    {

        animator.Play("AppearFromBelow");
        //gameObject.SetActive(true);
    }

    IEnumerator PlayAnimation(string animationName)
    {
        animator.Play(animationName);
        yield return null;
    }
}
