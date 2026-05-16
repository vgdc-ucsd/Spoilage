using System;
using TMPro;
using UnityEngine;

public class DayTimer : Singleton<DayTimer> {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private TextMeshProUGUI _textMeshPro;
    [SerializeField] float _startUITime = 9.00f;
    [SerializeField] float _endUITime = 17.00f; //Military time
    [SerializeField] float _realWorldMinutes = 10.0f;
    private Boolean isCounting = false;
    private float currentMilitaryTime;
    private float secondsPassed = 0.0f;
    private float clockIncrementTime;
    void Start()
    {
        clockIncrementTime = Instance._realWorldMinutes * 60 / (Instance._endUITime - Instance._startUITime);
        currentMilitaryTime = Instance._startUITime;
        _textMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
         _textMeshPro.text = _startUITime + ":00 AM";
    }

    public void startTimer()
    {
        currentMilitaryTime = _startUITime;
        secondsPassed = 0.0f;
        isCounting = true;
    }

    // Update is called once per frame
    void Update()
    {   
        if(isCounting){
            secondsPassed += Time.deltaTime;
            if(secondsPassed >= _realWorldMinutes * 60)
            {
                //TODO: End day function
                isCounting = false;
            }
            if (Mathf.Floor(secondsPassed / clockIncrementTime) + _startUITime > currentMilitaryTime)
            {
                _textMeshPro.text = string.Empty;
                currentMilitaryTime += 1;
                if(currentMilitaryTime > 11)
                {
                    
                    _textMeshPro.text = (currentMilitaryTime - 1) % 12 + 1  + ":00 PM";
                } else
                {
                    _textMeshPro.text = currentMilitaryTime + ":00 AM";
                }    
            }
        }    
    }
}
