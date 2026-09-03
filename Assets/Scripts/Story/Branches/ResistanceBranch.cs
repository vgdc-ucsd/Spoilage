using UnityEngine;

[CreateAssetMenu(fileName = "ResistanceBranch", menuName = "Progression/Branch/Resistance")]
public class ResistanceBranch : Branch<Interactions>
{
    [SerializeField] private float _threshold;
    [SerializeField] private GraphNode<Interactions> _belowThreshold;
    [SerializeField] private GraphNode<Interactions> _aboveThreshold;

    public override GraphNode<Interactions> Next()
    {
        // TODO
        return _belowThreshold;
    }
}
