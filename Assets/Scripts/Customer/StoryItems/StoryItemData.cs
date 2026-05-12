using UnityEngine;

[CreateAssetMenu(fileName = "NewStoryItem", menuName = "ScriptableObjects/StoryItemData")]
public class StoryItemData : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _sprite;

    public string Name => _name;
    public Sprite Sprite => _sprite;
}
