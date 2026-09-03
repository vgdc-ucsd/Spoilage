using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialoguePlayer : MonoBehaviour
{
    private const int MAX_VISIBLE_LINES = 3;
    private const float MAX_BUBBLE_WAIT_SECONDS = 3.0f;
    private const float FADE_TIME = MAX_BUBBLE_WAIT_SECONDS / 5.0f;
    private const float BUBBLE_TIME = MAX_BUBBLE_WAIT_SECONDS / 10.0f;
    private const float BUBBLE_SPACING = 0f;
    private const int CHARACTER_LIMIT = 150;

    [SerializeField] private RectTransform _dialogueSpawnpoint;
    [SerializeField] private DialogueBubble _bubblePrefab;    
    private LinkedList<DialogueBubble> _bubbles = new LinkedList<DialogueBubble>();
    private Action _callback;

    public void PlayDialogue(List<string> dialogue, Action callback)
    {
        StopAllCoroutines();
        ClearDialogue();
        _callback = callback;
        StartCoroutine(BubbleDialogue(dialogue));
    }

    void Update()
    {
        if (DebugManager.Instance.AllowSkipDialogue && Keyboard.current.sKey.wasPressedThisFrame)
        {
            StopAllCoroutines();
            ClearDialogue();
        }
    }

    private void ClearDialogue()
    {
        foreach (Transform child in _dialogueSpawnpoint)
        {
            Destroy(child.gameObject);
        }
        _bubbles.Clear();
        _callback?.Invoke();
        _callback = null;
    }

    // Ranges from half to full max wait time depending on the length of the line
    // Feel free to tweak
    private float CalculateWaitTime(int numCharacters)
    {
        float halfTime = MAX_BUBBLE_WAIT_SECONDS / 2.0f;
        float additionalWaitSeconds = Mathf.Lerp(0f, halfTime, (float)numCharacters / CHARACTER_LIMIT);
        return halfTime + additionalWaitSeconds;
    }

    private IEnumerator BubbleDialogue(List<string> dialogue)
    {
        foreach (string line in dialogue)
        {
            float startTime = Time.time;

            DialogueBubble bubble = Instantiate(_bubblePrefab);
            bubble.transform.SetParent(_dialogueSpawnpoint);
            bubble.transform.localScale = Vector3.one;
            bubble.SetText(line);
            bubble.FadeIn(FADE_TIME);
            _bubbles.AddFirst(bubble);

            if (_bubbles.Count > MAX_VISIBLE_LINES)
            {
                _bubbles.Last.Value.FadeOut(FADE_TIME);
            }

            while (Time.time - startTime < BUBBLE_TIME)
            {    
                int i = 0;
                float bubbleHeight = 0;

                foreach (DialogueBubble b in _bubbles)
                {
                    if (b == null) continue;
                    Vector2 size = b.GetSize(); 
                    b.transform.localPosition = new Vector2(size.x, bubbleHeight);
                    bubbleHeight += size.y + BUBBLE_SPACING;
                    i++;
                }

                yield return null;
            }
            
            if (_bubbles.Count > MAX_VISIBLE_LINES)
            {
                _bubbles.RemoveLast();
            }

            float waitTime = CalculateWaitTime(bubble.CharacterCount());
            while (Time.time - startTime < waitTime)
            {
                yield return null;
            }
        }

        // Fade out remaining bubbles
        float baseBubbleHeight = 0;
        while (_bubbles.Count > 0)
        {   
            int i = 0;
            float bubbleHeight = 0;
            foreach (DialogueBubble b in _bubbles)
            {
                if (b == null) continue;
                Vector2 size = b.GetSize();

                if (i == 0)
                {
                    baseBubbleHeight += size.y + BUBBLE_SPACING;
                }

                b.transform.localPosition = new Vector2(size.x, bubbleHeight) + new Vector2(0f, baseBubbleHeight);
                bubbleHeight += size.y + BUBBLE_SPACING;
                i++;
            }

            if (_bubbles.Last != null && _bubbles.Last.Value != null)
            {
                _bubbles.Last.Value.FadeOut(FADE_TIME);
            }
            _bubbles.RemoveLast();
            yield return new WaitForSeconds(MAX_BUBBLE_WAIT_SECONDS / 4.0f);
        }

        ClearDialogue();
    }
}
