using UnityEngine;

public sealed class StoryItem
{
    public StoryItemData Data { get; private set; }

    public StoryItem(StoryItemData data)
    {
        Data = data;
    }

    public void ChangeData(StoryItemData newData)
    {
        Data = newData;
    }
}