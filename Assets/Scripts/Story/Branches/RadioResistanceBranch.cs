using UnityEngine;

[CreateAssetMenu(fileName = "RadioResistanceBranch", menuName = "Progression/Branch/RadioResistance")]
public class RadioResistanceBranch : Branch<TextAsset>
{
    [SerializeField] private float _threshold;
    [SerializeField] private GraphNode<TextAsset> _belowThreshold;
    [SerializeField] private GraphNode<TextAsset> _aboveThreshold;

    public override GraphNode<TextAsset> Next()
    {
        return SaveManager.Instance.Player.resistanceScore > _threshold ? _aboveThreshold : _belowThreshold;
    }
}