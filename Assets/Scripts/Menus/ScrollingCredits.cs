using UnityEngine;
using UnityEngine.UI;

public class ScrollingCredits : MonoBehaviour
{
    [SerializeField] private float _scrollSpeed = 30f;
    private RectTransform _rectTransform;
    private float _scrollHeight;
    private bool _finishedScrolling = false;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _scrollHeight = _rectTransform.rect.height;
    }

    void Update()
    {
        if (_finishedScrolling) return;

        _rectTransform.anchoredPosition += new Vector2(0, _scrollSpeed * Time.deltaTime);

        // Checks to see if the credits image has scrolled entirely off the top of screen
        // This only works if the image RectTransform pivot Y value is set to 1 and is anchored to the top
        if (_rectTransform.anchoredPosition.y >= _scrollHeight)
        {
            _finishedScrolling = true;
            OnFinishScroll();
        }
    }

    private void OnFinishScroll()
    {
        Debug.Log("Finished scrolling credits.");
    }
}
