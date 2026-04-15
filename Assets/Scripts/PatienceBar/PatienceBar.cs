using System;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UI;

public class PatienceBar : MonoBehaviour
{
    // wowow
    private const float Urgent = 0.2f;

    [SerializeField] private Slider _slider;
    private bool _isUrgent;
    private float _patience;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _isUrgent = false;
    }

    // Update is called once per frame
    void Update()
    {
        _slider.value = _patience;
        if (_slider.normalizedValue < Urgent || !_isUrgent)
        {
            _isUrgent = true;
            _slider.fillRect.GetComponent<Image>().color = Color.red;
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
    /// Number of milliseconds between each interval
    /// </param>
    public void beginDecay(int rate, int interval) {
        
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
