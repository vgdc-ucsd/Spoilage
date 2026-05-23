using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CustomerAnimation : MonoBehaviour
{
    public bool isBlinking { get; private set; }
    public bool isTalking { get; private set; }
    [SerializeField] private Image eyesOpenRenderer;
    [SerializeField] private Image eyesClosedRenderer;
    [SerializeField] private Image eyesDisgustRenderer;
    [SerializeField] private Image eyesAngerRenderer;
    [SerializeField] private Image eyesWideningRenderer;
    [SerializeField] private Image mouthOpenRenderer;
    [SerializeField] private Image mouthClosedRenderer;
    [SerializeField] private Image mouthDisgustRenderer;
    [SerializeField] private Image mouthAngerRenderer;
    [SerializeField] private Image spoilageBackRenderer1;
    [SerializeField] private Image spoilageBackRenderer2;
    [SerializeField] private Image spoilageFrontRenderer1;
    [SerializeField] private Image spoilageFrontRenderer2;
    

    [SerializeField] private Mood currentMood;
    [SerializeField] private SpoilageStatus currentSpoilageStatus;
    [SerializeField] public float currentBlinkMultiplier = 1;

    private Image currentEyesRenderer;
    private Image currentMouthRenderer;

    private const float MIN_BLINK_TIME = 0.1f;
    private const float MAX_BLINK_TIME = 0.4f;
    private const float MIN_BLINK_COOLDOWN = 2.0f;
    private const float MAX_BLINK_COOLDOWN = 10.0f;

    private const float SPOILAGE_ANIM_TIME = 0.2f;

    public enum Mood
    {
        NEUTRAL,
        DISGUST,
        ANGER,
        WIDENING
    }

    public enum SpoilageStatus
    {
        OFF,
        FRAME_1,
        FRAME_2
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isBlinking = true;
        isTalking = false;
        currentEyesRenderer = eyesOpenRenderer;
        currentMouthRenderer = mouthClosedRenderer;
        SetMood(Mood.NEUTRAL);
        SetSpoilageStatus(SpoilageStatus.OFF);
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
            yield return new WaitForSeconds(GetBlinkCooldown());
            if (isBlinking)
            {
                SetOpenEyes(false);
            }
            yield return new WaitForSeconds(GetBlinkTime());
            if (isBlinking)
            {
                SetOpenEyes(true);
            }
        }
    }

    private float GetBlinkCooldown()
    {
        return Random.Range(MIN_BLINK_COOLDOWN, MAX_BLINK_COOLDOWN) * currentBlinkMultiplier;
    }
    private float GetBlinkTime()
    {
        return Random.Range(MIN_BLINK_TIME, MAX_BLINK_TIME) * currentBlinkMultiplier;
    }

    public void StartSpoilageAnim()
    {
        StartCoroutine(AnimateSpoilage());
    }
    private IEnumerator AnimateSpoilage()
    {
        while (true)
        {
            SetSpoilageStatus(SpoilageStatus.FRAME_1);
            yield return new WaitForSeconds(SPOILAGE_ANIM_TIME);
            SetSpoilageStatus(SpoilageStatus.FRAME_2);
            yield return new WaitForSeconds(SPOILAGE_ANIM_TIME);
        }
    }

    public void SetSpoilageStatus(SpoilageStatus stat)
    {
        currentSpoilageStatus = stat;
        switch(currentSpoilageStatus)
        {
            case SpoilageStatus.OFF:
                spoilageBackRenderer1.enabled = false;
                spoilageBackRenderer2.enabled = false;
                spoilageFrontRenderer1.enabled = false;
                spoilageFrontRenderer2.enabled = false;
                break;
            case SpoilageStatus.FRAME_1:
                spoilageBackRenderer1.enabled = true;
                spoilageBackRenderer2.enabled = false;
                spoilageFrontRenderer1.enabled = true;
                spoilageFrontRenderer2.enabled = false;
                break;
            case SpoilageStatus.FRAME_2:
                spoilageBackRenderer1.enabled = false;
                spoilageBackRenderer2.enabled = true;
                spoilageFrontRenderer1.enabled = false;
                spoilageFrontRenderer2.enabled = true;
                break;
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
