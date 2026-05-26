using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private DialoguePlayer _dialoguePlayer;
    [SerializeField] private DialogueRegistry _dialogueFiles;

    public void PlayDialogue(List<string> dialogue, Action callback)
    {
        _dialoguePlayer.PlayDialogue(dialogue, callback);
    }

    public CustomerDialogue LoadCustomerDialogue(string filePath)
    {
        TextAsset dialogueFile = Resources.Load<TextAsset>(filePath);
        return JsonUtility.FromJson<CustomerDialogue>(dialogueFile.text);
    }

    public CustomerDialogue SelectGeneralDialogue(CustomerData data)
    {
        int day = 0;
        StoryRoute route = StoryRoute.ANY;
        AppearanceTime appearance = AppearanceTime.UNSPECIFIED;

        if (SaveManager.Instance?.Player != null)
        {
            day = SaveManager.Instance.Player.Day; 
            route = SaveManager.Instance.Player.resistanceScore > 7.0f ? StoryRoute.RESISTANCE : StoryRoute.WARLORD;
        }

        if (day <= 10) appearance = AppearanceTime.EARLY;
        else if (day <= 20) appearance = AppearanceTime.MIDDLE;
        else appearance = AppearanceTime.LATE;

        List<DialogueEntry> entries = _dialogueFiles.DialogueEntries.FindAll(entry => 
            (entry.Route == route || entry.Route == StoryRoute.ANY) &&
            (entry.Appearence == appearance || entry.Appearence == AppearanceTime.UNSPECIFIED) &&
            entry.Spoilage == data.spoilage
        );

        if (entries.Count == 0)
        {
            Debug.LogError($"No matching dialogue entries could be found for Spoilage: {data.spoilage}, Day {day}, Route: {route}, Appearance: {appearance}");
            return null;
        }

        DialogueEntry entry = entries[UnityEngine.Random.Range(0, entries.Count)];
        CustomerDialogue dialogue = JsonUtility.FromJson<CustomerDialogue>(entry.DialogueFile.text);
        InsertDishName(dialogue, data);

        return dialogue;
    }

    private List<string> InsertDishName(List<string> lines, CustomerData data)
    {
        return lines.Select(
            line => Regex.Replace(line, @"\[DISH\]", data.orders[0].name)
        ).ToList();
    }

    private void InsertDishName(CustomerDialogue dialogue, CustomerData data)
    {
        dialogue.Intro = InsertDishName(dialogue.Intro, data);
        dialogue.Success = InsertDishName(dialogue.Success, data);
        dialogue.Fail = InsertDishName(dialogue.Fail, data);
        dialogue.Reject = InsertDishName(dialogue.Reject, data);
    }
}
