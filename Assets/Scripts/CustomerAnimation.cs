using System.Collections;
using UnityEngine;

public class CustomerAnimation : MonoBehaviour
{
    public bool isBlinking { get; private set; }
    public bool isTalking { get; private set; }
    [SerializeField] private Animator _characterAnimator;

    private const float MIN_BLINK_TIME = 0.1f;
    private const float MAX_BLINK_TIME = 0.4f;
    private const float MIN_BLINK_COOLDOWN = 2.0f;
    private const float MAX_BLINK_COOLDOWN = 10.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isBlinking = true;
        isTalking = false;
        StartCoroutine(RandomBlinking());
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

    public void SetOpenEyes(bool open)
    {
        _characterAnimator.SetBool("OpenEyes", open);
    }

    public void SetBlinking(bool blink)
    {
        isBlinking = blink;
    }

    public void SetOpenMouth(bool open)
    {
        _characterAnimator.SetBool("OpenMouth", open);

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
