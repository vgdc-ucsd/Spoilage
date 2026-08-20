using UnityEngine;

[CreateAssetMenu(fileName = "NewStation", menuName = "Stations/StationData")]
public class StationData : ScriptableObject
{
    [SerializeField] StationType _stationType;

    public StationType StationType => _stationType;
    public string StationName => StationLookup.GetName(_stationType);
}
