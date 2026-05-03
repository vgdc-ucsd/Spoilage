using UnityEngine;

public class LogicManager : MonoBehaviour
{
    public GameObject button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResizeButton()
    {
        button.transform.localScale *= 0.75f;
    }
}
