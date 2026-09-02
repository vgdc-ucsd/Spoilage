using TMPro;
using UnityEngine;

public class DayTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tmp;

    private int _minutes;
    private int _hours;
    private bool _counting;
    private bool _dayOver;
    private float _timer;

    private const int START_HOUR = 9;
    private const int END_HOUR = 17;
    private const float REAL_SECONDS_PER_GAME_HOUR = 15f;
    private const float TIMER_BLINK_SPEED = 2f;
    private const float TIMER_BLINK_INTENSITY = 0.3f;

    public void StartTimer()
    {
        _timer = 0f;
        _counting = true;
        _dayOver = false;
    }

    public void StopTimer()
    {
        _counting = false;
        _tmp.color = Color.white;
    }

    void Start()
    {
        _counting = false;
        _dayOver = false;
        _tmp.color = Color.white;
        _minutes = 0;
        _hours = START_HOUR;
        UpdateText();
    }

    private void UpdateText()
    {
        int hoursAmPm = _hours == 12 ? 12 : _hours % 12;
        string amPm = _hours >= 12 ? "PM" : "AM";
        _tmp.text = $"{hoursAmPm}:{_minutes:D2} {amPm}";

        if (_dayOver)
        {
            float t = Mathf.Cos((_timer - ((END_HOUR - START_HOUR) * REAL_SECONDS_PER_GAME_HOUR)) * TIMER_BLINK_SPEED); // range (-1, 1) starting at 1
            t *= -1f; // range (-1, 1) starting at -1
            t = (t + 1.0f) / 2.0f; // range (0, 1) starting at 0
            t *= TIMER_BLINK_INTENSITY; // range (0, blink) starting at 0
            _tmp.color = Color.Lerp(Color.white, Color.black, t);
        }
    }

    private void EndDay()
    {
        _minutes = 0;
        _dayOver = true;
        SetupManager.Instance.SetTimeLimitReached();
    }

    void Update()
    {  
        if (!_counting) return; 

        _timer += Time.deltaTime;
        _hours = Mathf.Clamp(
            START_HOUR + Mathf.FloorToInt(_timer / REAL_SECONDS_PER_GAME_HOUR),
            START_HOUR,
            END_HOUR 
        );

        if (_hours == END_HOUR)
        {
            if (!_dayOver) EndDay();
        }
        else
        {
            _minutes = Mathf.FloorToInt(_timer % REAL_SECONDS_PER_GAME_HOUR / REAL_SECONDS_PER_GAME_HOUR * 60f);
        }

        UpdateText();
    }
}
