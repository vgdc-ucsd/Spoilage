using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class HoverableButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _rectTransform.localScale *= 1.05f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _rectTransform.localScale /= 1.05f;
    }
}
