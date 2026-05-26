using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBubble : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _bubbleText;
    [SerializeField] private Image _background;
    [SerializeField] private RectTransform _rt;

    public void SetText(string text)
    {
        // Insert a line break at the end of a word every 50 characters
        string result = Regex.Replace(
            text,
            @"(.{1,50})(\s+|$)",
            "$1" + Environment.NewLine
        );

        _bubbleText.text = result;
        _bubbleText.ForceMeshUpdate();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rt);
    }

    public Vector2 GetSize()
    {
        return _bubbleText.GetPreferredValues();
    }

    public int CharacterCount()
    {
        return _bubbleText.GetParsedText().Length;
    }

    public void FadeIn(float duration)
    {
        StartCoroutine(FadeInRoutine(duration));
    }

    public void FadeOut(float duration)
    {
        StartCoroutine(FadeOutRoutine(duration));
    }

    private IEnumerator FadeInRoutine(float duration)
    {
        yield return BasicAnimations.Interpolate
        (
            null,
            (t) =>
            {
                float ease = BasicAnimations.EaseOut(t);
                _background.color = new Color(1.0f, 1.0f, 1.0f, ease);
                _bubbleText.color = new Color
                (
                    _bubbleText.color.r, 
                    _bubbleText.color.g,
                    _bubbleText.color.b, 
                    ease
                );
            },
            () =>
            {
                _background.color = Color.white;
                _bubbleText.color = new Color
                (
                    _bubbleText.color.r, 
                    _bubbleText.color.g,
                    _bubbleText.color.b, 
                    1.0f
                );
            },
            duration
        );
    }

    private IEnumerator FadeOutRoutine(float duration)
    {
        yield return BasicAnimations.Interpolate
        (
            null,
            (t) =>
            {
                float ease = BasicAnimations.EaseOut(t);
                _background.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - ease);
                _bubbleText.color = new Color
                (
                    _bubbleText.color.r, 
                    _bubbleText.color.g,
                    _bubbleText.color.b, 
                    1.0f - ease
                );
            },
            null,
            duration
        );

        Destroy(gameObject);
    }
}
