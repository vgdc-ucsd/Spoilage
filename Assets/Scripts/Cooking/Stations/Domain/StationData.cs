using UnityEngine;

public enum StationType
{
    Automatic,
    Manual
}

[CreateAssetMenu(fileName = "NewStation", menuName = "Stations/StationData")]
public class StationData : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private StationCategory _stationCategory;
    [SerializeField] private StationType _stationType;
    [SerializeField] private bool _overcook;
    [SerializeField] private Sprite _spriteOff;
    [SerializeField] private Sprite _spriteOn;

    public StationCategory StationCategory => _stationCategory;
    public string CategoryName => StationLookup.CategoryName(_stationCategory);
    public StationType StationType => _stationType;
    public string Name => _name;
    public bool Overcook => _overcook;
    public Sprite SpriteOff => _spriteOff;
    public Sprite SpriteOn => _spriteOn;
}
