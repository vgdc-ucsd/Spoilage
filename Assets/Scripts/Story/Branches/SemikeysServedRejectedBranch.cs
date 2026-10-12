using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SemikeysServedRejectedBranch", menuName = "Progression/Branch/SemikeysServedRejected")]
public class SemikeysServedRejectedBranch : Branch<Interactions>
{
    [SerializeField] private List<string> _servedSemikeyIds;
    [SerializeField] private List<string> _rejectedSemikeyIds;
    [SerializeField] private GraphNode<Interactions> _meetsCondition;
    [SerializeField] private GraphNode<Interactions> _doesNotMeetCondition;

    public override GraphNode<Interactions> Next()
    {
        foreach (string servedId in _servedSemikeyIds)
        {
            if (!SaveManager.Instance.Player.SeenSemikeyCharacters.Contains(servedId))
            {
                return _doesNotMeetCondition;
            }
        }

        foreach (string servedId in _servedSemikeyIds)
        {
            if (SaveManager.Instance.Player.RejectedSemikeyCharacters.Contains(servedId))
            {
                return _doesNotMeetCondition;
            }
        }

        foreach (string rejectedId in _rejectedSemikeyIds)
        {
            if (!SaveManager.Instance.Player.RejectedSemikeyCharacters.Contains(rejectedId))
            {
                return _doesNotMeetCondition;
            }
        }

        return _meetsCondition;
    }
}
