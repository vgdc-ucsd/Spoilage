using UnityEngine;

[CreateAssetMenu(fileName = "Save", menuName = "Progression/DebugSave")]
public class DebugPlayerData : ScriptableObject
{
    [SerializeField] private PlayerData _playerData;
    public PlayerData PlayerData => _playerData;
}

public class DebugManager : Singleton<DebugManager>
{
#if UNITY_EDITOR
    private const bool DEBUG = true;
#else
    private const bool DEBUG = false;
#endif
    [SerializeField] private DebugPlayerData _debugPlayerSave;

    public PlayerData DebugPlayerSave => DEBUG ? _debugPlayerSave?.PlayerData : null;
}
