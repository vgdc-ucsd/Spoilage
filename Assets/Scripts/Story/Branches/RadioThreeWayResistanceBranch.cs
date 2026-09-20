using UnityEngine;

[CreateAssetMenu(fileName = "RadioThreeWayResistanceBranch", menuName = "Progression/Branch/RadioThreeWayResistance")]
public class RadioThreeWayResistanceBranch : Branch<TextAsset>
{
    [SerializeField] private float _upperThreshold;
    [SerializeField] private float _lowerThreshold;
    [SerializeField] private GraphNode<TextAsset> _belowLowerThreshold;
    [SerializeField] private GraphNode<TextAsset> _middleThreshold;
    [SerializeField] private GraphNode<TextAsset> _aboveUpperThreshold;

    public override GraphNode<TextAsset> Next()
    {
        float score = SaveManager.Instance.Player.resistanceScore;
        if (score < _lowerThreshold) return _belowLowerThreshold;
        if (score > _upperThreshold) return _aboveUpperThreshold;
        return _middleThreshold;
    }
}