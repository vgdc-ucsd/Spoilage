using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePlayer : MonoBehaviour
{
    private const int MAX_VISIBILE_LINES = 3;
    private const float MAX_BUBBLE_WAIT_SECONDS = 3.0f;
    private const float FADE_TIME = MAX_BUBBLE_WAIT_SECONDS / 5.0f;
    private const float BUBBLE_TIME = MAX_BUBBLE_WAIT_SECONDS / 10.0f;
    private const float BUBBLE_SPACING = 0f;
    private const int CHARACTER_LIMIT = 150;

    [SerializeField] private RectTransform _dialogueSpawnpoint;
    [SerializeField] private DialogueBubble _bubblePrefab;    
    private LinkedList<DialogueBubble> _bubbles = new LinkedList<DialogueBubble>();

    public void PlayDialogue(List<string> dialogue, Action callback)
    {
        StopAllCoroutines();
        ClearDialogue();
        StartCoroutine(BubbleDialogue(dialogue, callback));
    }

    private void ClearDialogue()
    {
        foreach (Transform child in _dialogueSpawnpoint)
        {
            Destroy(child.gameObject);
        }
    }

    // Ranges from half to full max wait time depending on the length of the line
    // Feel free to tweak
    private float CalculateWaitTime(int numCharacters)
    {
        float halfTime = MAX_BUBBLE_WAIT_SECONDS / 2.0f;
        float additionalWaitSeconds = Mathf.Lerp(0f, halfTime, (float)numCharacters / CHARACTER_LIMIT);
        return halfTime + additionalWaitSeconds;
    }

    private IEnumerator BubbleDialogue(List<string> dialogue, Action callback)
    {
        Vector3[] corners = new Vector3[4];
        _dialogueSpawnpoint.GetWorldCorners(corners);
        Vector2 spawnBottomLeft = corners[0];
        
        foreach (string line in dialogue)
        {
            float startTime = Time.time;

            DialogueBubble bubble = Instantiate(_bubblePrefab);
            bubble.transform.SetParent(_dialogueSpawnpoint);
            bubble.SetText(line);
            bubble.FadeIn(FADE_TIME);
            _bubbles.AddFirst(bubble);

            if (_bubbles.Count > MAX_VISIBILE_LINES)
            {
                _bubbles.Last.Value.FadeOut(FADE_TIME);
            }

            while (Time.time - startTime < BUBBLE_TIME)
            {    
                int i = 0;
                float bubbleHeight = 0;

                foreach (DialogueBubble b in _bubbles)
                {
                    Vector2 size = b.GetSize();
                    b.transform.position = new Vector2(size.x, bubbleHeight) + spawnBottomLeft;
                    bubbleHeight += size.y + BUBBLE_SPACING;
                    i++;
                }

                yield return null;
            }
            
            if (_bubbles.Count > MAX_VISIBILE_LINES)
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
                Vector2 size = b.GetSize();

                if (i == 0)
                {
                    baseBubbleHeight += size.y + BUBBLE_SPACING;
                }

                b.transform.position = new Vector2(size.x, bubbleHeight) + spawnBottomLeft + new Vector2(0f, baseBubbleHeight);
                bubbleHeight += size.y + BUBBLE_SPACING;
                i++;
            }

            _bubbles.Last.Value.FadeOut(FADE_TIME);
            _bubbles.RemoveLast();
            yield return new WaitForSeconds(MAX_BUBBLE_WAIT_SECONDS / 4.0f);
        }

        ClearDialogue();
        callback?.Invoke();
    }
}
