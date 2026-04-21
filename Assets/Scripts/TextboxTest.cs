using System.Collections.Generic;
using TextboxControl;
using UnityEngine;
using UnityEngine.InputSystem;

public class TextboxTest : MonoBehaviour
{
    public TextAsset SaveFileAsset;
    public string SequenceName = "wavy_intro";

    TextboxController controller;
    DialogueSaveFile saveFile;
    int boxIndex;

    private void Start()
    {
        controller = GetComponent<TextboxController>();
        saveFile = DialogueSaveFile.Parse(SaveFileAsset.text);

        PlayBox(0);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Advance();
        }
    }

    void Advance()
    {
        if (controller.IsRevealing)
        {
            controller.Skip();
            return;
        }

        PlayBox(boxIndex + 1);
    }

    void PlayBox(int index)
    {
        boxIndex = index;
        controller.Play(saveFile.GetBox(SequenceName, boxIndex));
    }
}
