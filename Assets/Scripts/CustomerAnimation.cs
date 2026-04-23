using System.Collections;
using UnityEngine;

public class CustomerAnimation : MonoBehaviour
{
    public bool isBlinking { get; private set; }
    public bool isTalking { get; private set; }
    [SerializeField] private SpriteRenderer eyesOpenRenderer;
    [SerializeField] private SpriteRenderer eyesClosedRenderer;
    [SerializeField] private SpriteRenderer eyesDisgustRenderer;
    [SerializeField] private SpriteRenderer eyesAngerRenderer;
    [SerializeField] private SpriteRenderer eyesWideningRenderer;
    [SerializeField] private SpriteRenderer mouthOpenRenderer;
    [SerializeField] private SpriteRenderer mouthClosedRenderer;
    [SerializeField] private SpriteRenderer mouthDisgustRenderer;
    [SerializeField] private SpriteRenderer mouthAngerRenderer;

    [SerializeField] private Mood currentMood;

    private SpriteRenderer currentEyesRenderer;
    private SpriteRenderer currentMouthRenderer;

    private const float MIN_BLINK_TIME = 0.1f;
    private const float MAX_BLINK_TIME = 0.4f;
    private const float MIN_BLINK_COOLDOWN = 2.0f;
    private const float MAX_BLINK_COOLDOWN = 10.0f;

    public enum Mood
    {
        NEUTRAL,
        DISGUST,
        ANGER,
        WIDENING
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isBlinking = true;
        isTalking = false;
        currentEyesRenderer = eyesOpenRenderer;
        currentMouthRenderer = mouthClosedRenderer;
        SetMood(Mood.NEUTRAL);
        StartCoroutine(RandomBlinking());
    }

    [ContextMenu("Update Mood")]
    private void updateMood()
    {
        SetMood(currentMood);
    }

    private IEnumerator RandomBlinking()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(MIN_BLINK_COOLDOWN, MAX_BLINK_COOLDOWN));
            if (isBlinking)
            {
                SetOpenEyes(false);
            }
            yield return new WaitForSeconds(Random.Range(MIN_BLINK_TIME, MAX_BLINK_TIME));
            if (isBlinking)
            {
                SetOpenEyes(true);
            }
        }
    }

    public void SetMood(Mood mood)
    {
        currentMood = mood;
        SetEyeMood(mood);
        SetMouthMood(mood);
    }

    public void SetEyeMood(Mood mood)
    {
        switch (mood)
        {
            case Mood.DISGUST:
                currentEyesRenderer = eyesDisgustRenderer;
                break;
            case Mood.ANGER:
                currentEyesRenderer = eyesAngerRenderer;
                break;
            case Mood.WIDENING:
                currentEyesRenderer = eyesWideningRenderer;
                break;
            default:
                currentEyesRenderer = eyesOpenRenderer;
                break;
        }
        SetOpenEyes(true);  // force update to new eye mood
    }

    public void SetMouthMood(Mood mood)
    {
        switch (mood)
        {
            case Mood.DISGUST:
                currentMouthRenderer = mouthDisgustRenderer;
                break;
            case Mood.ANGER:
                currentMouthRenderer = mouthAngerRenderer;
                break;
            default:
                currentMouthRenderer = mouthClosedRenderer;
                break;
        }
        if (!isTalking)
        {
            SetOpenMouth(false);  // force update to new mouth mood if mouth closed
        }
    }

    public void SetOpenEyes(bool open)
    {
        ResetEyes();
        if (open)
        {
            if (currentEyesRenderer.sprite != null)
            {
                currentEyesRenderer.enabled = true;
            }
            else
            {
                Debug.LogWarning("Attempted to use eyes with missing sprite! Defaulting to neutral eyes.");
                eyesOpenRenderer.enabled = true;
            }
        }
        else
        {
            eyesClosedRenderer.enabled = true;
        }
    }

    private void ResetEyes()
    {
        eyesOpenRenderer.enabled = false;
        eyesClosedRenderer.enabled = false;
        eyesDisgustRenderer.enabled = false;
        eyesAngerRenderer.enabled = false;
        eyesWideningRenderer.enabled = false;
    }

    public void SetBlinking(bool blink)
    {
        isBlinking = blink;
    }

    private void SetOpenMouth(bool open)
    {
        ResetMouth();
        if (open)
        {
            mouthOpenRenderer.enabled = true;
        }
        else
        {
            if (currentMouthRenderer.sprite != null)
            {
                currentMouthRenderer.enabled = true;
            }
            else
            {
                Debug.LogWarning("Attempted to use mouth with missing sprite! Defaulting to neutral mouth.");
                mouthClosedRenderer.enabled = true;
            }
        }
    }

    private void ResetMouth()
    {
        mouthOpenRenderer.enabled = false;
        mouthClosedRenderer.enabled = false;
        mouthDisgustRenderer.enabled = false;
        mouthAngerRenderer.enabled = false;
    }

    [ContextMenu("Toggle Blinking")]
    private void ToggleBlinking()
    {
        isBlinking = !isBlinking;
        SetBlinking(isBlinking);
        Debug.Log("Blinking: " + isBlinking.ToString());
    }

    [ContextMenu("Toggle Mouth Open")]
    private void ToggleMouthOpen()
    {
        isTalking = !isTalking;
        SetOpenMouth(isTalking);
        Debug.Log("Mouth Open: " + isTalking.ToString());
    }
}

/*
Types of eyes:
- blinking
- disgust
- anger
- widening
- static

Types of mouths:
- talking
- disgust
- anger
- static
*/