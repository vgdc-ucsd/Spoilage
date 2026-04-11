using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Slider TimerSlider;
    public Text TimerText;
    public float TimeRemaining;
    private bool _stopTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _stopTimer = false;
        TimerSlider.maxValue = TimeRemaining;
        TimerSlider.value = TimeRemaining;
    }

    // Update is called once per frame
    private void Update()
    {
        float time = TimeRemaining - Time.time;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time - minutes * 60f);
        string textTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        if (time <= 0)
        {
            _stopTimer = true;
        }
        if (_stopTimer == false)
        {
            TimerText.text = textTime;
            TimerSlider.value = time;
        }
    }
}