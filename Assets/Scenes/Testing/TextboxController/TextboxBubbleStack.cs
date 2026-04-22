using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TextboxControl;

public enum TextboxBubbleSide
{
    L,
    R
}

public class TextboxBubbleStack : MonoBehaviour
{
    static readonly TMP_Text[] EmptyTexts = System.Array.Empty<TMP_Text>();
    static readonly float[] EmptyTextAlphas = System.Array.Empty<float>();
    static readonly SpriteRenderer[] EmptySprites = System.Array.Empty<SpriteRenderer>();
    static readonly Color[] EmptySpriteColors = System.Array.Empty<Color>();

    public GameObject BubblePrefab;
    public Transform BubbleParent;
    public int MaxBubbles = 5;

    public TextboxBubbleSide DefaultSide = TextboxBubbleSide.R;

    public float LeftAnchorX = -3.2f;
    public float RightAnchorX = 3.2f;
    public float OppositeSideGap = 1.35f;

    public Vector3 FirstBubblePosition = new Vector3(0f, -3.1f, 0f);
    public Vector3 StackDirection = Vector3.up;
    public Vector3 AppearOffset = new Vector3(0f, -0.35f, 0f);
    public Vector3 DisappearOffset = new Vector3(0f, 0.45f, 0f);

    public float VisibleHeight = 5.7f;
    public float BubbleSpacing = 0.18f;
    public float MaxTextWidth = 4.1f;
    public float TargetLineHeight = 0.42f;
    public Vector2 BubblePadding = new Vector2(0.38f, 0.24f);
    public Vector2 MinBubbleSize = new Vector2(1.1f, 0.62f);

    public bool CreateBackground = true;
    public string BackgroundObjectName = "BubbleBackground";
    public Color BackgroundColor = new Color(1f, 1f, 1f, 0.9f);

    public float AppearDuration = 0.22f;
    public float MoveDuration = 0.22f;
    public float DisappearDuration = 0.22f;

    static Sprite _backgroundSprite;

    readonly List<Bubble> _bubbles = new List<Bubble>();

    private class Bubble
    {
        public Transform Root;
        public TextboxController Controller;
        public TextboxBubbleSide Side;
        public BubbleVisuals Visuals;
        public Coroutine MoveCoroutine;
        public Vector2 Size;
    }

    private class BubbleVisuals
    {
        public TMP_Text[] Texts;
        public float[] TextAlphas;
        public SpriteRenderer[] Sprites;
        public Color[] SpriteColors;
        public SpriteRenderer Background;
    }

    public TextboxController Push(string source)
    {
        return Push(source, DefaultSide);
    }

    public TextboxController Push(string source, TextboxBubbleSide side)
    {
        if (BubblePrefab == null)
        {
            Debug.LogError("[TextboxBubbleStack] BubblePrefab not assigned.", this);
            return null;
        }

        if (!TryCreateBubble(source, side, out Bubble bubble))
        {
            return null;
        }

        PrepareBubble(bubble, true);
        SetAlpha(bubble.Visuals, 0f);

        _bubbles.Add(bubble);
        TrimBubbles();

        int index = _bubbles.Count - 1;
        Vector3 targetPosition = GetPosition(index);
        bubble.Root.localPosition = targetPosition + AppearOffset;
        bubble.MoveCoroutine = StartCoroutine(FadeAndMove(
            bubble,
            bubble.Root.localPosition,
            targetPosition,
            0f,
            1f,
            AppearDuration));

        LayoutBubbles(true, bubble);

        return bubble.Controller;
    }

    bool TryCreateBubble(string source, TextboxBubbleSide side, out Bubble bubble)
    {
        bubble = null;

        Transform parent = BubbleParent != null ? BubbleParent : transform;
        GameObject instance = Instantiate(BubblePrefab, parent);
        TextboxController controller = instance.GetComponentInChildren<TextboxController>(true);
        if (controller == null)
        {
            Debug.LogError("[TextboxBubbleStack] BubblePrefab must contain a TextboxController.", instance);
            Destroy(instance);
            return false;
        }

        controller.Play(source);

        bubble = new Bubble
        {
            Root = instance.transform,
            Controller = controller,
            Side = side,
        };
        return true;
    }

    public void RefreshLayout(bool animate)
    {
        for (int i = 0; i < _bubbles.Count; i++)
        {
            PrepareBubble(_bubbles[i], false);
        }

        TrimBubbles();
        LayoutBubbles(animate, null);
    }

    public void SetAnchors(float leftAnchorX, float rightAnchorX, float oppositeSideGap, bool animate)
    {
        LeftAnchorX = leftAnchorX;
        RightAnchorX = rightAnchorX;
        OppositeSideGap = oppositeSideGap;
        RefreshLayout(animate);
    }

    public void SetSide(TextboxController controller, TextboxBubbleSide side, bool animate)
    {
        for (int i = 0; i < _bubbles.Count; i++)
        {
            Bubble bubble = _bubbles[i];
            if (bubble != null && bubble.Controller == controller)
            {
                bubble.Side = side;
                PrepareBubble(bubble, true);
                LayoutBubbles(animate, null);
                return;
            }
        }
    }

    void TrimBubbles()
    {
        int maxBubbles = Mathf.Max(1, MaxBubbles);
        while (_bubbles.Count > maxBubbles)
        {
            RemoveOldestBubble();
        }

        while (VisibleHeight > 0f && _bubbles.Count > 1 && GetStackHeight() > VisibleHeight)
        {
            RemoveOldestBubble();
        }
    }

    void RemoveOldestBubble()
    {
        Bubble oldest = _bubbles[0];
        _bubbles.RemoveAt(0);
        StartCoroutine(FadeOutAndDestroy(oldest));
    }

    float GetStackHeight()
    {
        if (_bubbles.Count == 0)
        {
            return 0f;
        }

        float height = BubbleSpacing * (_bubbles.Count - 1);
        for (int i = 0; i < _bubbles.Count; i++)
        {
            height += _bubbles[i].Size.y;
        }

        return height;
    }

    void LayoutBubbles(bool animate, Bubble skip)
    {
        for (int i = 0; i < _bubbles.Count; i++)
        {
            Bubble bubble = _bubbles[i];
            if (bubble == skip || bubble.Root == null)
            {
                continue;
            }

            Vector3 targetPosition = GetPosition(i);
            if (!animate || MoveDuration <= 0f)
            {
                bubble.Root.localPosition = targetPosition;
                continue;
            }

            if (bubble.MoveCoroutine != null)
            {
                StopCoroutine(bubble.MoveCoroutine);
            }

            bubble.MoveCoroutine = StartCoroutine(MoveBubble(bubble, targetPosition));
        }
    }

    Vector3 GetPosition(int index)
    {
        if (index < 0 || index >= _bubbles.Count)
        {
            return FirstBubblePosition;
        }

        Vector3 direction = StackDirection.sqrMagnitude > 0f ? StackDirection.normalized : Vector3.up;
        Vector3 position = FirstBubblePosition;

        for (int i = _bubbles.Count - 2; i >= index; i--)
        {
            Bubble lower = _bubbles[i + 1];
            Bubble upper = _bubbles[i];

            float lowerHeight = lower != null ? lower.Size.y : MinBubbleSize.y;
            float upperHeight = upper != null ? upper.Size.y : MinBubbleSize.y;

            position += direction * ((lowerHeight + upperHeight) * 0.5f + BubbleSpacing);
        }

        position.x = GetHorizontalPosition(_bubbles[index]);
        return position;
    }

    float GetHorizontalPosition(Bubble bubble)
    {
        if (bubble.Side == TextboxBubbleSide.L)
        {
            return LeftAnchorX + bubble.Size.x * 0.5f;
        }

        return RightAnchorX - bubble.Size.x * 0.5f;
    }

    void PrepareBubble(Bubble bubble, bool renderNow)
    {
        if (bubble == null || bubble.Root == null)
        {
            return;
        }

        if (bubble.Visuals == null)
        {
            if (CreateBackground)
            {
                EnsureBackground(bubble.Root);
            }

            bubble.Visuals = CaptureVisuals(bubble.Root);
        }
        else if (CreateBackground && bubble.Visuals.Background == null)
        {
            EnsureBackground(bubble.Root);
            bubble.Visuals = CaptureVisuals(bubble.Root);
        }

        bubble.Size = MeasureBubble(bubble.Visuals.Texts);
        ResizeBackground(bubble.Visuals.Background, bubble.Size);

        bubble.Controller.NotifyTextLayoutChanged(renderNow);
    }

    Vector2 MeasureBubble(TMP_Text[] texts)
    {
        float maxBubbleWidth = Mathf.Max(
            MinBubbleSize.x,
            RightAnchorX - LeftAnchorX - Mathf.Max(0f, OppositeSideGap));

        float maxTextWidth = Mathf.Max(0.01f, maxBubbleWidth - BubblePadding.x * 2f);
        if (MaxTextWidth > 0f)
        {
            maxTextWidth = Mathf.Min(maxTextWidth, MaxTextWidth);
        }

        Vector2 textSize = Vector2.zero;

        if (texts != null)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                TMP_Text text = texts[i];
                if (text == null)
                {
                    continue;
                }

                text.enableWordWrapping = true;
                text.overflowMode = TextOverflowModes.Overflow;

                float scale = GetTextScale(text);
                Vector3 currentScale = text.transform.localScale;
                if (!Mathf.Approximately(currentScale.x, scale) || !Mathf.Approximately(currentScale.y, scale))
                {
                    text.transform.localScale = new Vector3(scale, scale, currentScale.z);
                }

                float localMaxTextWidth = maxTextWidth / scale;

                Vector2 natural = text.GetPreferredValues(text.text, 100000f, Mathf.Infinity);
                float localTextWidth = Mathf.Min(natural.x, localMaxTextWidth);
                localTextWidth = Mathf.Max(0.01f, localTextWidth);

                Vector2 wrapped = text.GetPreferredValues(text.text, localTextWidth, Mathf.Infinity);
                float localTextHeight = Mathf.Max(0.01f, wrapped.y);

                if (text.rectTransform != null)
                {
                    text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localTextWidth);
                    text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localTextHeight);
                }

                text.ForceMeshUpdate(true, true);

                textSize.x = Mathf.Max(textSize.x, localTextWidth * scale);
                textSize.y = Mathf.Max(textSize.y, localTextHeight * scale);
            }
        }

        float bubbleWidth = Mathf.Clamp(
            textSize.x + BubblePadding.x * 2f,
            MinBubbleSize.x,
            maxBubbleWidth);

        float bubbleHeight = Mathf.Max(
            MinBubbleSize.y,
            textSize.y + BubblePadding.y * 2f);

        return new Vector2(bubbleWidth, bubbleHeight);
    }

    float GetTextScale(TMP_Text text)
    {
        if (text == null)
        {
            return 1f;
        }

        Vector2 line = text.GetPreferredValues("Mg", Mathf.Infinity, Mathf.Infinity);
        float lineHeight = Mathf.Max(0.01f, line.y);
        float targetLineHeight = Mathf.Max(0.01f, TargetLineHeight);

        return targetLineHeight / lineHeight;
    }

    void EnsureBackground(Transform root)
    {
        if (FindBackground(root) != null)
        {
            return;
        }

        GameObject backgroundObject = new GameObject(BackgroundObjectName);
        backgroundObject.transform.SetParent(root, false);
        backgroundObject.transform.localPosition = new Vector3(0f, 0f, 0.1f);

        SpriteRenderer background = backgroundObject.AddComponent<SpriteRenderer>();
        background.sprite = GetBackgroundSprite();
        background.drawMode = SpriteDrawMode.Sliced;
        background.color = BackgroundColor;
        background.sortingOrder = -10;
    }

    void ResizeBackground(SpriteRenderer background, Vector2 size)
    {
        if (background == null)
        {
            return;
        }

        background.color = BackgroundColor;

        if (background.drawMode == SpriteDrawMode.Simple)
        {
            Vector2 spriteSize = background.sprite != null ? background.sprite.bounds.size : Vector2.one;
            background.transform.localScale = new Vector3(
                spriteSize.x > 0f ? size.x / spriteSize.x : size.x,
                spriteSize.y > 0f ? size.y / spriteSize.y : size.y,
                background.transform.localScale.z);
        }
        else
        {
            background.size = size;
        }
    }

    SpriteRenderer FindBackground(Transform root)
    {
        if (root == null || string.IsNullOrEmpty(BackgroundObjectName))
        {
            return null;
        }

        Transform background = root.Find(BackgroundObjectName);
        if (background == null)
        {
            return null;
        }

        return background.GetComponent<SpriteRenderer>();
    }

    IEnumerator MoveBubble(Bubble bubble, Vector3 targetPosition)
    {
        if (bubble == null || bubble.Root == null)
        {
            yield break;
        }

        Transform target = bubble.Root;
        Vector3 startPosition = target.localPosition;
        float elapsed = 0f;

        while (elapsed < MoveDuration)
        {
            if (target == null)
            {
                yield break;
            }

            elapsed += Time.deltaTime;
            target.localPosition = Vector3.Lerp(startPosition, targetPosition, Mathf.Clamp01(elapsed / MoveDuration));
            yield return null;
        }

        if (target != null)
        {
            target.localPosition = targetPosition;
        }

        if (bubble != null)
        {
            bubble.MoveCoroutine = null;
        }
    }

    IEnumerator FadeAndMove(Bubble bubble, Vector3 startPosition, Vector3 endPosition, float startAlpha, float endAlpha, float duration)
    {
        if (bubble == null || bubble.Root == null)
        {
            yield break;
        }

        Transform target = bubble.Root;

        if (duration <= 0f)
        {
            target.localPosition = endPosition;
            SetAlpha(bubble.Visuals, endAlpha);
            bubble.MoveCoroutine = null;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (target == null)
            {
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.localPosition = Vector3.Lerp(startPosition, endPosition, t);
            SetAlpha(bubble.Visuals, Mathf.Lerp(startAlpha, endAlpha, t));
            yield return null;
        }

        if (target != null)
        {
            target.localPosition = endPosition;
        }

        SetAlpha(bubble.Visuals, endAlpha);
        bubble.MoveCoroutine = null;
    }

    IEnumerator FadeOutAndDestroy(Bubble bubble)
    {
        if (bubble == null || bubble.Root == null)
        {
            yield break;
        }

        if (bubble.MoveCoroutine != null)
        {
            StopCoroutine(bubble.MoveCoroutine);
            bubble.MoveCoroutine = null;
        }

        Transform target = bubble.Root;
        Vector3 startPosition = target.localPosition;
        Vector3 endPosition = startPosition + DisappearOffset;

        if (DisappearDuration > 0f)
        {
            float elapsed = 0f;
            while (elapsed < DisappearDuration)
            {
                if (target == null)
                {
                    yield break;
                }

                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / DisappearDuration);
                target.localPosition = Vector3.Lerp(startPosition, endPosition, t);
                SetAlpha(bubble.Visuals, 1f - t);
                yield return null;
            }
        }

        if (target != null)
        {
            Destroy(target.gameObject);
        }
    }

    BubbleVisuals CaptureVisuals(Transform target)
    {
        BubbleVisuals visuals = new BubbleVisuals();
        if (target == null)
        {
            visuals.Texts = EmptyTexts;
            visuals.TextAlphas = EmptyTextAlphas;
            visuals.Sprites = EmptySprites;
            visuals.SpriteColors = EmptySpriteColors;
            visuals.Background = null;
            return visuals;
        }

        visuals.Texts = target.GetComponentsInChildren<TMP_Text>(true);
        visuals.TextAlphas = new float[visuals.Texts.Length];
        for (int i = 0; i < visuals.Texts.Length; i++)
        {
            visuals.TextAlphas[i] = visuals.Texts[i].alpha;
        }

        visuals.Sprites = target.GetComponentsInChildren<SpriteRenderer>(true);
        visuals.SpriteColors = new Color[visuals.Sprites.Length];
        for (int i = 0; i < visuals.Sprites.Length; i++)
        {
            visuals.SpriteColors[i] = visuals.Sprites[i].color;
        }

        visuals.Background = FindBackground(target);

        return visuals;
    }

    static void SetAlpha(BubbleVisuals visuals, float alpha)
    {
        if (visuals == null)
        {
            return;
        }

        for (int i = 0; i < visuals.Texts.Length; i++)
        {
            if (visuals.Texts[i] != null)
            {
                visuals.Texts[i].alpha = visuals.TextAlphas[i] * alpha;
            }
        }

        for (int i = 0; i < visuals.Sprites.Length; i++)
        {
            if (visuals.Sprites[i] != null)
            {
                Color color = visuals.SpriteColors[i];
                color.a *= alpha;
                visuals.Sprites[i].color = color;
            }
        }
    }

    static Sprite GetBackgroundSprite()
    {
        if (_backgroundSprite != null)
        {
            return _backgroundSprite;
        }

        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        _backgroundSprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, 1f, 1f),
            new Vector2(0.5f, 0.5f),
            100f);

        return _backgroundSprite;
    }
}
