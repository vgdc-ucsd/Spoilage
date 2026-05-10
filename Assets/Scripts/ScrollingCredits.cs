using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingCredits : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 30f;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
    }
}
