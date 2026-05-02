using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueTimer : MonoBehaviour
{
    TextMeshProUGUI dialogueText;

    [TextArea]
    public string[] dialogueLines;

    public float timeBetweenLines = 3f;

    void Start()
    {
        // Automatically grab the text component
        dialogueText = GetComponent<TextMeshProUGUI>();

        StartCoroutine(PlayDialogue());
    }

    IEnumerator PlayDialogue()
    {
        while(true)
        {
            for (int i = 0; i < dialogueLines.Length; i++)
            {
                dialogueText.text = dialogueLines[i];

                yield return new WaitForSeconds(timeBetweenLines);
            }

        }
        
    }
}