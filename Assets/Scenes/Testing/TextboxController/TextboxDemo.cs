using System.Collections;
using TextboxControl;
using UnityEngine;

public class TextboxDemo : MonoBehaviour
{
    public TextAsset SaveFileAsset;
    public string SequenceName = "wavy_intro";
    public TextboxBubbleStack BubbleStack;

    public float AdvanceDelay = 0.35f;
    public bool Loop;
    public bool AlternateSides = true;
    public TextboxBubbleSide FirstSide = TextboxBubbleSide.L;

    private DialogueSaveFile _saveFile;
    private int _boxIndex;
    private int _boxCount;
    private TextboxController _current;
    private Coroutine _autoAdvanceCoroutine;

    private void Start()
    {
        if (!TryResolveBubbleStack(out TextboxBubbleStack stack))
        {
            return;
        }

        if (!TryParseSaveFile(out DialogueSaveFile saveFile))
        {
            return;
        }

        int boxCount = saveFile.CountBoxes(SequenceName);
        if (boxCount <= 0)
        {
            Debug.LogError($"[TextboxDemo] Sequence '{SequenceName}' not found or empty.", this);
            return;
        }

        BubbleStack = stack;
        _saveFile = saveFile;
        _boxCount = boxCount;

        PlayBox(0);
    }

    private bool TryResolveBubbleStack(out TextboxBubbleStack stack)
    {
        stack = BubbleStack != null ? BubbleStack : GetComponent<TextboxBubbleStack>();
        if (stack == null)
        {
            Debug.LogError("[TextboxDemo] BubbleStack not assigned.", this);
            return false;
        }

        return true;
    }

    private bool TryParseSaveFile(out DialogueSaveFile saveFile)
    {
        saveFile = null;

        if (SaveFileAsset == null)
        {
            Debug.LogError("[TextboxDemo] SaveFileAsset not assigned.", this);
            return false;
        }

        try
        {
            saveFile = DialogueSaveFile.Parse(SaveFileAsset.text);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[TextboxDemo] Failed to parse save file: {e.Message}", this);
            return false;
        }
    }

    private void PlayBox(int index)
    {
        _boxIndex = index;

        string source = _saveFile.GetBox(SequenceName, _boxIndex);
        TextboxBubbleSide side = GetSide(_boxIndex);
        TextboxController controller = BubbleStack.Push(source, side);
        if (controller == null)
        {
            return;
        }

        _current = controller;

        if (_autoAdvanceCoroutine != null)
        {
            StopCoroutine(_autoAdvanceCoroutine);
        }

        _autoAdvanceCoroutine = StartCoroutine(AutoAdvance(controller));
    }

    private TextboxBubbleSide GetSide(int index)
    {
        if (!AlternateSides)
        {
            return FirstSide;
        }

        bool firstIsLeft = FirstSide == TextboxBubbleSide.L;
        bool isEven = (index & 1) == 0;
        return isEven == firstIsLeft ? TextboxBubbleSide.L : TextboxBubbleSide.R;
    }

    private IEnumerator AutoAdvance(TextboxController controller)
    {
        while (controller != null && controller.IsRevealing)
        {
            yield return null;
        }

        yield return new WaitForSeconds(AdvanceDelay);

        if (controller == _current)
        {
            Advance();
        }

        _autoAdvanceCoroutine = null;
    }

    private void Advance()
    {
        int nextIndex = _boxIndex + 1;
        if (nextIndex < _boxCount)
        {
            PlayBox(nextIndex);
            return;
        }

        if (Loop)
        {
            PlayBox(0);
        }
    }
}
