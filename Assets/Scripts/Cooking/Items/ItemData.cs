using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private string _id;
    [SerializeField] private Sprite _sprite;

    public string Name => _name;
    public string ID => _id;
    public Sprite ItemSprite => _sprite;
}
