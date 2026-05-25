using System.Collections.Generic;
using UnityEngine;

public class ExampleDialogue : MonoBehaviour
{
    void Start()
    {
        CustomerDialogue dialogue = DialogueManager.Instance.LoadCustomerDialogue("Dialogue/Example/ExampleDialogue");
        DialogueManager.Instance.PlayDialogue(
            dialogue.Intro, 
            () => Debug.Log($"Dialogue Finished! This is where you could trigger receiving {dialogue.Item.ID}")
        );
    }
}
