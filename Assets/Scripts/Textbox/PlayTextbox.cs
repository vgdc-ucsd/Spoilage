using UnityEngine;
using TextboxControl;

public class PlayTextbox : MonoBehaviour
{
    public TextboxController textbox;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        textbox.Play("Hello!");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
