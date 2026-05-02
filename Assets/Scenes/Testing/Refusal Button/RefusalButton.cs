using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class RefusalButton : MonoBehaviour
{
    public Animator anim;
    public UnityEvent buttonPress;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonPress.AddListener(GameObject.FindGameObjectWithTag("LogicManager").GetComponent<LogicManager>().ResizeButton);
        buttonPress.AddListener(AnimateButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        Debug.Log("Button Pressed");
        buttonPress.Invoke();
    }

    public void AnimateButton()
    {
        anim.SetTrigger("Button Pressed");
    }
}
