using System.Collections.Generic;
using TextboxControl;
using UnityEngine;

public class TextboxTest : MonoBehaviour
{
    [TextArea(2, 5)]
    public string Source = "\x1b[12;80\x1cHello, world! This is the textbox engine.";

    Reducer reducer;
    int lastRevealed;

    void Start() {
        reducer = new Reducer();
        reducer.OnComplete += () => Debug.Log("[TextboxTest] Reveal complete.");
        reducer.OnError += msg => Debug.LogWarning("[TextboxTest] " + msg);
        reducer.Play(Source);
        lastRevealed = 0;
    }

    void Update()
    {
        reducer.Tick(Time.deltaTime);

        int revealed = reducer.RevealedCount;
        if (revealed == lastRevealed) return;

        lastRevealed = revealed;
        List<char> chars = new List<char>(reducer.DisplayBuffer);
        Debug.Log("[TextboxTest] " + new string(chars.ToArray()));
    }
}
