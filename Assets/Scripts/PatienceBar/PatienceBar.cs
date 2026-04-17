using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UI;

/// <summary>
/// Visual element displaying the current patience level of the customer. Bar 
/// can have its minimum and maximum value adjusted depending on the customer 
/// using the Init method as well as dedicated setter methods for the max and 
/// min. The current patience value may also be adjusted. Patience decays at a
/// constant rate and is activated with the beginDecay method and can be stopped
/// with stopDecay. PatienceBar contains two UnityEvents for when the patience
/// reaches below a certain threshold (PatienceIsUrgent) to be considered 
/// "urgent" as well as an event (PatienceExpired) for when patience reaches 0.
/// </summary>
public class PatienceBar : MonoBehaviour
{
    // wowow
    // Threshold for warning, currently arbitrary
    private const float UrgentDefault = 0.2f;
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
        if (_slider.normalizedValue <= UrgentDefault && !_isUrgent)
        {
            _isUrgent = true;
            // this line will probably change once they decide what its gonna be
            _slider.fillRect.GetComponent<Image>().color = Color.red;
            PatienceIsUrgent.Invoke();
        }

        // removes urgent status if above threshold
        if (_slider.normalizedValue > UrgentDefault && _isUrgent)
        {
            _isUrgent = false;
            // this line will probably change once they decide what its gonna be
            _slider.fillRect.GetComponent<Image>().color = Color.white;
        }
    }

    /// <summary>
    /// Initializes the PatienceBar with the max and minimum values.
    /// If patience is not given, will be set to max by default
    /// </summary>
    /// <param name="minPatience">Min value for bar</param>
    /// <param name="maxPatience">Max value for bar</param>
    public void Init(float minPatience, float maxPatience)
    {
        _slider.maxValue = maxPatience;
        _slider.minValue = minPatience;
        _patience = maxPatience;
    }

    /// <summary>
    /// Initializes the PatienceBar with the max and minimum values, then sets
    /// patience to given value
    /// </summary>
    /// <param name="minPatience">Min value for bar</param>
    /// <param name="maxPatience">Max value for bar</param>
    /// <param name="patience">Starting patience value</param>
    public void Init(float minPatience, float maxPatience, float patience)
    {
        _slider.maxValue = maxPatience;
        _slider.minValue = minPatience;
        _patience = patience;
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

    /// <summary>
    /// Getter for current patience value
    /// </summary>
    /// <returns>Patience as float</returns>
    public float getPatience()
    {
        return _patience;
    }

    /// <summary>
    /// Getter for max patience value
    /// </summary>
    /// <returns>Max as float</returns>
    public float getPatienceMax()
    {
        return _slider.maxValue;
    }

    /// <summary>
    /// Getter for min patience value
    /// </summary>
    /// <returns>Min as float</returns>
    public float getPatienceMin()
    {
        return _slider.maxValue;
    }

    /// <summary>
    /// Setter for max patience value
    /// </summary>
    /// <param name="max">float</param>
    /// <exception cref="ArgumentOutOfRangeException">max should be > min</exception>
    public void setPatienceMax(float max)
    {
        if (max <= _slider.minValue)
        {
            throw new ArgumentOutOfRangeException("Max should be greater than min");
        }
        _slider.maxValue = max;
    }

    /// <summary>
    /// Setter for min patience value
    /// </summary>
    /// <param name="min">float</param>
    /// <exception cref="ArgumentOutOfRangeException">max should be < min</exception>
    public void setPatienceMin(float min)
    {
        if (min >= _slider.minValue)
        {
            throw new ArgumentOutOfRangeException("Min should be less than max");
        }
        _slider.maxValue = min;
    }
}
