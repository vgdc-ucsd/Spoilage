using UnityEngine;

/// <summary>
/// Root ScripableObject that aggregates every authoring asset the story system
/// reads.
/// </summary>
[CreateAssetMenu(fileName = "StoryDatabase", menuName = "Story/Story Database", order = -1)]
public class StoryDatabase : ScriptableObject
{
    public CharacterRoster roster;
    public DayTimeline timeline;
    public RadioBroadcasts radio;
    public ReactionaryRules reactionary;
    public Endings endings;
    public StoryItems items;
    public RandomDialoguePools randomDialogue;
}
