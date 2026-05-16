using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CustomerAnimation : MonoBehaviour
{
    public bool isBlinking { get; private set; }
    public bool isTalking { get; private set; }
    [SerializeField] private Image _eyesOpenRenderer;
    [SerializeField] private Image _eyesClosedRenderer;
    [SerializeField] private Image _eyesDisgustRenderer;
    [SerializeField] private Image _eyesAngerRenderer;
    [SerializeField] private Image _eyesWideningRenderer;
    [SerializeField] private Image _mouthOpenRenderer;
    [SerializeField] private Image _mouthClosedRenderer;
    [SerializeField] private Image _mouthDisgustRenderer;
    [SerializeField] private Image _mouthAngerRenderer;

    [SerializeField] private Mood _currentMood;

    private Image _currentEyesRenderer;
    private Image _currentMouthRenderer;

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
        _currentEyesRenderer = _eyesOpenRenderer;
        _currentMouthRenderer = _mouthClosedRenderer;
        SetMood(Mood.NEUTRAL);
        StartCoroutine(RandomBlinking());
    }

    [ContextMenu("Update Mood")]
    private void updateMood()
    {
        SetMood(_currentMood);
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
        _currentMood = mood;
        SetEyeMood(mood);
        SetMouthMood(mood);
    }

    public void SetEyeMood(Mood mood)
    {
        switch (mood)
        {
            case Mood.DISGUST:
                _currentEyesRenderer = _eyesDisgustRenderer;
                break;
            case Mood.ANGER:
                _currentEyesRenderer = _eyesAngerRenderer;
                break;
            case Mood.WIDENING:
                _currentEyesRenderer = _eyesWideningRenderer;
                break;
            default:
                _currentEyesRenderer = _eyesOpenRenderer;
                break;
        }
        SetOpenEyes(true);  // force update to new eye mood
    }

    public void SetMouthMood(Mood mood)
    {
        switch (mood)
        {
            case Mood.DISGUST:
                _currentMouthRenderer = _mouthDisgustRenderer;
                break;
            case Mood.ANGER:
                _currentMouthRenderer = _mouthAngerRenderer;
                break;
            default:
                _currentMouthRenderer = _mouthClosedRenderer;
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
            if (_currentEyesRenderer.sprite != null)
            {
                _currentEyesRenderer.enabled = true;
            }
            else
            {
                Debug.LogWarning("Attempted to use eyes with missing sprite! Defaulting to neutral eyes.");
                _eyesOpenRenderer.enabled = true;
            }
        }
        else
        {
            _eyesClosedRenderer.enabled = true;
        }
    }

    private void ResetEyes()
    {
        _eyesOpenRenderer.enabled = false;
        _eyesClosedRenderer.enabled = false;
        _eyesDisgustRenderer.enabled = false;
        _eyesAngerRenderer.enabled = false;
        _eyesWideningRenderer.enabled = false;
    }

    public void SetBlinking(bool blink)
    {
        isBlinking = blink;
    }

    public void SetTalking(bool talking)
    {
        isTalking = talking;
        SetOpenMouth(talking);
    }

    public void ApplyMouthState(string state)
    {
        switch (state)
        {
            case "OPEN":
                SetMouthMood(Mood.NEUTRAL);
                SetTalking(true);
                break;
            case "ANGER":
                SetMouthMood(Mood.ANGER);
                SetTalking(false);
                break;
            case "DISGUST":
                SetMouthMood(Mood.DISGUST);
                SetTalking(false);
                break;
            default: // CLOSED
                SetMouthMood(Mood.NEUTRAL);
                SetTalking(false);
                break;
        }
    }

    public void ApplyEyeState(string state)
    {
        switch (state)
        {
            case "CLOSED":
                isBlinking = false;
                SetOpenEyes(false);
                break;
            case "ANGER":
                isBlinking = false;
                SetEyeMood(Mood.ANGER);
                break;
            case "DISGUST":
                isBlinking = false;
                SetEyeMood(Mood.DISGUST);
                break;
            case "WIDENING":
                isBlinking = false;
                SetEyeMood(Mood.WIDENING);
                break;
            default: // OPEN
                isBlinking = true;
                SetEyeMood(Mood.NEUTRAL);
                break;
        }
    }

    private void SetOpenMouth(bool open)
    {
        ResetMouth();
        if (open)
        {
            _mouthOpenRenderer.enabled = true;
        }
        else
        {
            if (_currentMouthRenderer.sprite != null)
            {
                _currentMouthRenderer.enabled = true;
            }
            else
            {
                Debug.LogWarning("Attempted to use mouth with missing sprite! Defaulting to neutral mouth.");
                _mouthClosedRenderer.enabled = true;
            }
        }
    }

    private void ResetMouth()
    {
        _mouthOpenRenderer.enabled = false;
        _mouthClosedRenderer.enabled = false;
        _mouthDisgustRenderer.enabled = false;
        _mouthAngerRenderer.enabled = false;
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
// We should integrate animation to Customer.cs so we can automatically assign facial expression sprite autmoatically.


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