using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Layer : MonoBehaviour
{
    public int totalLayers; // holy shit this is SO bad the idx of the layer increases as the layers go deeper but we need to store total layers anyways for the sorting layers HIGH HIGH HIGH HIGH CORITSOL NEVER DOING ANOTHER GAME JAM WHILE HAVING IMPORTANT TESTS EVER AGAIN
    public SortingGroup sortingGroup;
    
    public int startingLayer;
    public bool isLastLayer; // and not to mention we wouldnt even need this flag if the idx went the other way

    SpriteRenderer[] renderers;
    float duration = 0.2f;

    Level level;

    private void Start()
    {
        level = FindFirstObjectByType<Level>();  
    }

    public void RefreshRenderers()
    {
        sortingGroup.sortingOrder = totalLayers - startingLayer;
        renderers = GetComponentsInChildren<SpriteRenderer>(); 
    }

    public void SetLayerVisible(bool isActive)
    {
        RefreshRenderers();
        SetAlpha(isActive ? 1 : 0);
    }

    public void TransitionLayers(bool isApproaching)
    {
        if (isLastLayer && level.IsWinning() && !isApproaching)
        {
            level.Win();
            //return;
        }
        StopAllCoroutines();
        FadeLayerVisible(isApproaching);
    }

    public void FadeLayerVisible(bool fadeIn)
    {
        RefreshRenderers();
        if (fadeIn)
        {
            StartCoroutine(FadeIn());
        } else
        {
            StartCoroutine(FadeOut());
        }
    } 

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            transform.localPosition = Vector3.up * Mathf.Lerp(-2f, 0f, progress);
            SetAlpha(Mathf.Lerp(0f, 1f, progress));
            yield return null;
        }
        transform.localPosition = Vector3.zero;
        SetAlpha(1f);
    }

    IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            transform.localPosition = Vector3.up * Mathf.Lerp(0f, 2f, progress);
            SetAlpha(Mathf.Lerp(1f, 0f, progress));
            yield return null;
        }
        transform.localPosition = Vector3.zero;
        SetAlpha(0f);
    }

    void SetAlpha(float alpha)
    {
        foreach (SpriteRenderer sr in renderers)
        {
            if (sr == null) continue; 
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }
}
