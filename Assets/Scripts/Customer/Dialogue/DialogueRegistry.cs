using System;
using System.Collections.Generic;
using UnityEngine;

public enum AppearanceTime
{
    UNSPECIFIED,
    EARLY,
    MIDDLE,
    LATE
}

public enum StoryRoute
{
    ANY,
    RESISTANCE,
    WARLORD,
}

[Serializable]
public class DialogueEntry
{
    public TextAsset DialogueFile;
    public CustomerData.Spoilage Spoilage;
    public AppearanceTime Appearence;
    public StoryRoute Route;
}

[CreateAssetMenu(menuName = "ScriptableObjects/DialogueRegistry")]
public class DialogueRegistry : ScriptableObject
{
    public List<DialogueEntry> DialogueEntries;
}
