using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UI;

public class PatienceBar : MonoBehaviour
{
    // wowow
    // Threshold for warning, currently arbitrary
    private const float Urgent = 0.2f;
    [SerializeField] private Slider _slider;
    private bool _isUrgent;
    private bool _isDecaying;
    private float _patience;
    private float _decayRate;
    private float _decayInterval;
    private float _timer;

    public UnityEvent PatienceExpired;
    public UnityEvent PatienceIsUrgent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PatienceExpired ??= new UnityEvent();
        PatienceIsUrgent ??= new UnityEvent();
        _patience = _slider.maxValue;
        beginDecay(5, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        // if decay is active, lower patience
        if (_isDecaying)
        {
            _timer = Time.deltaTime;
            _patience -= (_timer / _decayInterval) * _decayRate;
        }

        // if patience is at or below 0, end decay and invoke event
        if (_patience <= 0)
        {
            _patience = 0;
            _isDecaying = false;
            PatienceExpired.Invoke();
        }

        // updates slider value with current patience
        _slider.value = _patience;

        // if bar is below threshold make urgent
        if (_slider.normalizedValue <= Urgent && !_isUrgent)
        {
            _isUrgent = true;
            // this line will probably change once they decide what its gonna be
            _slider.fillRect.GetComponent<Image>().color = Color.red;
            PatienceIsUrgent.Invoke();
        }

        // removes urgent status if above threshold
        if (_slider.normalizedValue > Urgent && _isUrgent)
        {
            _isUrgent = false;
            // this line will probably change once they decide what its gonna be
            _slider.fillRect.GetComponent<Image>().color = Color.white;
        }
    }

    /// <summary>
    /// Initializes the PatienceBar with the max and minimum values
    /// </summary>
    /// <param name="minPatience">Max value for bar</param>
    /// <param name="maxPatience">Min value for bar</param>
    public void Init(float minPatience, float maxPatience)
    {
        _slider.maxValue = maxPatience;
        _slider.minValue = minPatience;
        _patience = maxPatience;
    }

    /// <summary>
    /// Decays patience by given rate each interval, with the number of 
    /// milliseconds between each interval given by the interval parameter
    /// </summary>
    /// <param name="rate">
    /// Number of patience points to reduce by each interval
    /// </param>
    /// <param name="interval">
    /// Number of seconds between each interval
    /// </param>
    public void beginDecay(float rate, float interval) {
        _isDecaying = true;
        _decayInterval = interval;
        _decayRate = rate;
    }

    /// <summary>
    /// Stops the decay of patience
    /// </summary>
    public void stopDecay()
    {
        _isDecaying = false;
    }

    /// <summary>
    /// Sets the current value of patience to value specified. Will throw an
    /// error if outside of min and max
    /// </summary>
    /// <param name="patience">New value of patience</param>
    /// <returns>Old value of patience</returns>
    public float setPatience(float patience)
    {
        if (patience > _slider.maxValue || patience < _slider.minValue)
        {
            throw new ArgumentOutOfRangeException();
        }
        float oldValue = _patience;
        _patience = patience;
        return oldValue;
    }
}
