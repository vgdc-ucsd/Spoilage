using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SemikeysRejectedThreeWayBranch", menuName = "Progression/Branch/SemikeysRejectedThreeWay")]
public class SemikeysRejectedThreeWayBranch : Branch<Interactions>
{
    [SerializeField] private string _primarySemikeyId;
    [SerializeField] private string _secondarySemikeyId;
    [SerializeField] private GraphNode<Interactions> _primaryServed;
    [SerializeField] private GraphNode<Interactions> _onlyPrimaryRejected;
    [SerializeField] private GraphNode<Interactions> _bothRejected;

    public override GraphNode<Interactions> Next()
    {
        if (!SaveManager.Instance.Player.RejectedSemikeyCharacters.Contains(_primarySemikeyId))
        {
            return _primaryServed;
        }
        else if (!SaveManager.Instance.Player.RejectedSemikeyCharacters.Contains(_secondarySemikeyId))
        {
            return _onlyPrimaryRejected;
        }
        else
        {
            return _bothRejected;
        }
    }
}
