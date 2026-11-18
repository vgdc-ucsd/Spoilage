using UnityEngine;

[CreateAssetMenu(fileName = "ItemGivenToSemikeyBranch", menuName = "Progression/Branch/ItemGivenToSemikey")]
public class ItemGivenToSemikeyBranch : Branch<Interactions>
{
    [SerializeField] private string _itemId;
    [SerializeField] private string _recipientId;
    [SerializeField] private GraphNode<Interactions> _itemGiven;
    [SerializeField] private GraphNode<Interactions> _itemNotGiven;

    public override GraphNode<Interactions> Next()
    {
        if (SaveManager.Instance.Player.ItemsGivenTo[_itemId] == _recipientId)
        {
            return _itemGiven;
        }

        return _itemNotGiven;
    }
}
