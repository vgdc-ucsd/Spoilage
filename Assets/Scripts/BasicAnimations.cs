using System;
using System.Collections;
using UnityEngine;

public static class BasicAnimations
{
    private const float C4 = 2 * Mathf.PI / 3;

    public static IEnumerator Interpolate(System.Action onStart, System.Action<float> tween, System.Action onEnd, float duration)
    {
        float t = 0;
        float startTime = Time.time;
        onStart?.Invoke();

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime = Time.time - startTime;
            t = elapsedTime / duration;
            tween.Invoke(t);
            yield return null;
        }

        onEnd?.Invoke();
    }

    // https://easings.net/
    public static float Smooth(float t) // Quadratic
    {
        t = t < 0.5f ? 2f * t * t : 1 - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
        return t;
    }

    public static float EaseIn(float t)
    {
        t = t * t;
        return t;
    }

    public static float EaseOut(float t)
    {
        t = 1f - (1f - t) * (1f - t);
        return t;
    }
    
    public static float EaseOutElastic(float t) {
        return t == 0f
            ? 0f
            : t == 1f
            ? 1f
            : Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * C4) + 1f;
    }

    public static float EaseInBack(float t) {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return c3 * t * t * t - c1 * t * t;
    }

    public static float EaseOutBack(float t) {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return (float)(1 + c3 * Math.Pow(t - 1, 3) + c1 * Math.Pow(t - 1, 2));
    }
}