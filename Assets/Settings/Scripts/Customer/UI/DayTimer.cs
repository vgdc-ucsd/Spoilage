using TMPro;
using UnityEngine;

public class DayTimer : Singleton<DayTimer> {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private TextMeshProUGUI _textMeshPro;
    [SerializeField] float _startTime = 9.00f;
    [SerializeField] float _endTime = 17.00f; //Military time
    [SerializeField] float _realWorldMinutes = 10.0f;
    private float currentMilitaryTime;
    private float secondsPassed = 0.0f;
    private float clockIncrementTime;
    void Start()
    {
        clockIncrementTime = Instance._realWorldMinutes * 60 / (Instance._endTime - Instance._startTime);
        currentMilitaryTime = Instance._startTime;
        _textMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
         _textMeshPro.text = _startTime + ":00 AM";
        
    }

    public void resetTimer()
    {
        currentMilitaryTime = _startTime;
        secondsPassed = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        secondsPassed += Time.deltaTime;
        if(secondsPassed >= _realWorldMinutes * 60)
        {
            //TODO: End day function
        }
        // if (secondsPassed % clockIncrementTime < epsilon)
        if (Mathf.Floor(secondsPassed / clockIncrementTime) + _startTime > currentMilitaryTime)
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
