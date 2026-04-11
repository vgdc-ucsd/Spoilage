using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Slider timerSlider;
    public Text timerText;
    public float timeRemaining;
    private bool stopTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stopTimer = false;
        timerSlider.maxValue = timeRemaining;
        timerSlider.value = timeRemaining;
    }

    // Update is called once per frame
    void Update()
    {
        float time = timeRemaining - Time.time;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time - minutes * 60f);
        string textTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        if (time <= 0)
        {
            stopTimer = true;
        }
        if (stopTimer == false)
        {
            timerText.text = textTime;
            timerSlider.value = time;
        }

        // if( onGrill )
        // {
        //     timeRemaining -= Time.deltaTime;
        // }

        // Debug.Log( (int) timeRemaining );
    }
}
