using System.Collections;
using UnityEngine;
using TextboxControl;

public class IMessageDemo : MonoBehaviour
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
        if (BubbleStack == null)
            BubbleStack = GetComponent<TextboxBubbleStack>();

        if (BubbleStack == null)
        {
            Debug.LogError("[IMessageDemo] BubbleStack not assigned.", this);
            return;
        }

        if (SaveFileAsset == null)
        {
            Debug.LogError("[IMessageDemo] SaveFileAsset not assigned.", this);
            return;
        }

        try
        {
            _saveFile = DialogueSaveFile.Parse(SaveFileAsset.text);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[IMessageDemo] Failed to parse save file: {e.Message}", this);
            return;
        }

        _boxCount = _saveFile.CountBoxes(SequenceName);
        if (_boxCount <= 0)
        {
            Debug.LogError($"[IMessageDemo] Sequence '{SequenceName}' not found or empty.", this);
            return;
        }

        PlayBox(0);
    }

    private void PlayBox(int index)
    {
        _boxIndex = index;

        string source = _saveFile.GetBox(SequenceName, _boxIndex);
        TextboxBubbleSide side = GetSide(_boxIndex);

        _current = BubbleStack.Push(source, side);
        if (_current == null)
            return;

        if (_autoAdvanceCoroutine != null)
            StopCoroutine(_autoAdvanceCoroutine);

        _autoAdvanceCoroutine = StartCoroutine(AutoAdvance(_current));
    }

    private TextboxBubbleSide GetSide(int index)
    {
        if (!AlternateSides)
            return FirstSide;

        bool firstIsLeft = FirstSide == TextboxBubbleSide.L;
        bool even = (index & 1) == 0;

        if (even == firstIsLeft)
            return TextboxBubbleSide.L;

        return TextboxBubbleSide.R;
    }

    private IEnumerator AutoAdvance(TextboxController controller)
    {
        while (controller != null && controller.IsRevealing)
            yield return null;

        yield return new WaitForSeconds(AdvanceDelay);

        if (controller == _current)
            Advance();

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
            PlayBox(0);
    }
}
