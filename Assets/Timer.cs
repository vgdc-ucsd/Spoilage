using UnityEngine;

public class Timer : MonoBehaviour
{
    bool onGrill;
    float timeRemaining = 60;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
