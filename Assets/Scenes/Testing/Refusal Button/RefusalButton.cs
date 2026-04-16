using UnityEngine;

public class RefusalButton : MonoBehaviour
{
    public Animator anim;
    public LogicManager logicManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        Debug.Log("Button Pressed");
        anim.SetTrigger("Button Pressed");
        logicManager.ResizeButton();
    }
}
