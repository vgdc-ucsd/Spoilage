using UnityEngine;

[CreateAssetMenu(fileName = "RejectedSemikeysWithSpecificBranch", menuName = "Progression/Branch/RejectedSemikeysWithSpecific")]
public class RejectedSemikeysWithSpecificBranch : Branch<Interactions>
{
    [SerializeField] private string _specificSemikeyId;
    [SerializeField] private GraphNode<Interactions> _onlySpecificRejected;
    [SerializeField] private GraphNode<Interactions> _allRejected;
    [SerializeField] private GraphNode<Interactions> _anyServed;

    public override GraphNode<Interactions> Next()
    {
        int seen = SaveManager.Instance.Player.SeenSemikeyCharacters.Count;
        int rejected = SaveManager.Instance.Player.RejectedSemikeyCharacters.Count;

        if (SaveManager.Instance.Player.RejectedSemikeyCharacters.Contains(_specificSemikeyId) && rejected == 1)
        {
            return _onlySpecificRejected;
        }
        if (rejected == 0) return _onlySpecificRejected;
        if (rejected == seen) return _allRejected;
        return _anyServed;
    }
}

