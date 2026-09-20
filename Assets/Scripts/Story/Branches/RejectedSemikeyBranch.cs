using UnityEngine;

[CreateAssetMenu(fileName = "RejectedSemikeyBranch", menuName = "Progression/Branch/RejectedSemikey")]
public class RejectedSemikeyBranch : Branch<Interactions>
{
    [SerializeField] private GraphNode<Interactions> _allRejected;
    [SerializeField] private GraphNode<Interactions> _someRejected;
    [SerializeField] private GraphNode<Interactions> _noneRejected;

    public override GraphNode<Interactions> Next()
    {
        int seen = SaveManager.Instance.Player.SeenSemikeyCharacters.Count;
        int rejected = SaveManager.Instance.Player.RejectedSemikeyCharacters.Count;

        if (rejected == 0) return _noneRejected;
        if (rejected == seen) return _allRejected;
        return _someRejected;
    }
}

