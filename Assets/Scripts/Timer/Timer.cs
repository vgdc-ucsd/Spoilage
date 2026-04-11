using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    bool onGrill;
    float timeRemaining = 60;
    public Slider timerSlider;
    public Text timerText;
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
        if( onGrill )
        {
            timeRemaining -= Time.deltaTime;
        }

        Debug.Log( (int) timeRemaining );
    }
}
