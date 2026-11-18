using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopTooltip : Singleton<ShopTooltip>
{
    [SerializeField] private TextMeshProUGUI _descriptionText;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Canvas _canvas;
    private RectTransform _canvasRect;

    public override void Awake()
    {
        base.Awake();

        _canvas = GetComponentInParent<Canvas>().rootCanvas;
        _canvasRect = (RectTransform)_canvas.transform;
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        Hide();
    }

    void Update()
    {
        if (_canvasGroup.alpha == 0f) return;
        FollowMouse();
    }

    public void Show(string text)
    {
        _descriptionText.text = text;
        _canvasGroup.alpha = 1f;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
        FollowMouse();
    }

    public void Hide()
    {
        _canvasGroup.alpha = 0f;
    }

    private void FollowMouse()
    {
        Vector2 mouse = Mouse.current.position.ReadValue();

        Camera cam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : _canvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect, mouse, cam, out Vector2 localPoint);

        Vector2 pos = localPoint;

        // Keep the tooltip on screen
        Vector2 size = _rectTransform.sizeDelta;
        Rect c = _canvasRect.rect;
        pos.x = Mathf.Min(pos.x, c.xMax - size.x);
        pos.y = Mathf.Max(pos.y, c.yMin + size.y);

        _rectTransform.localPosition = pos;
    }
}