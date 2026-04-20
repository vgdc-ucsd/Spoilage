using System.Collections.Generic;
using TextboxControl;
using UnityEngine;

public class TextboxTest : MonoBehaviour
{
    [TextArea(2, 5)]
    public string Source =
            "\x1b[12;60\x1c"
            + "\x1b[30;5;wavy;amp=3,freq=2\x1c"
            + "Hello "
            + "\x1b[30;5;rainbow\x1c"
            + "\x1b[30;5;jitter;amp=1\x1c"
            + "world!";

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
