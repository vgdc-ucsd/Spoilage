using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private DialoguePlayer _dialoguePlayer;

    public void PlayDialogue(List<string> dialogue, Action callback)
    {
        _dialoguePlayer.PlayDialogue(dialogue, callback);
    }

    public CustomerDialogue LoadCustomerDialogue(string filePath)
    {
        TextAsset dialogueFile = Resources.Load<TextAsset>(filePath);
        return JsonUtility.FromJson<CustomerDialogue>(dialogueFile.text);
    }
}
