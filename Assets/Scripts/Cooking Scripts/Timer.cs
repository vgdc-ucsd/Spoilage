using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Slider TimerSlider;
    public Text TimerText;
    private float _maxTime;
    public float TimeRemaining;
    private bool _stopTimer = true;
 
    public void StartTimer(float time)
    {
        gameObject.SetActive(true);

        _stopTimer = false;
        _maxTime = time;
        TimeRemaining = time;
        TimerSlider.maxValue = _maxTime;
        TimerSlider.value = TimeRemaining;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_stopTimer) return;

        TimeRemaining -= Time.deltaTime;
        float time = TimeRemaining;

        if (time < 0)
        {
            time = 0;
        }

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time - (minutes * 60f));
        string textTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        TimerText.text = textTime;
        TimerSlider.value = time;

        if (TimeRemaining <= 0)
        {
            _stopTimer = true;
            TimeRemaining = 0;
        }
    }

    public bool IsFinished()
    {
        return _stopTimer && _maxTime > 0 && TimeRemaining <= 0;
    }

    public void PauseTimer()
    {
        _stopTimer = true;
    }
    public void ResumeTimer()
    {
        // Only resume if there is actually time left
        if (TimeRemaining > 0)
        {
            _stopTimer = false;
        }
    }
}