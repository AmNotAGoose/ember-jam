using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI insideText;
    public string ogText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        insideText = GetComponentInChildren<TextMeshProUGUI>();
        ogText = insideText.text;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        insideText.SetText("> " + ogText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        insideText.SetText(ogText);
    }
}
