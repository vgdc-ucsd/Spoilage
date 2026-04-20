using System.Collections.Generic;
using TextboxControl;
using UnityEngine;

public class TextboxTest : MonoBehaviour
{
    [TextArea(2, 5)]
    public string Source = "\x1b[12;60\x1c\x1b[1;1\x1cHello\x1b[1;0\x1c, \x1b[5;F55\x1cworld\x1b[5;reset\x1c!";

    TextboxController _controller;

    void Start() {
        _controller = GetComponent<TextboxController>();
        if (_controller == null)
        {
            Debug.LogError("[TextboxTest] what (it broke again)");
            return;
        }

        _controller.OnComplete += () => Debug.Log("[TextboxTest] Reveal complete.");
        _controller.Play(Source);
    }
}
