using UnityEngine;

public class DebugManager : Singleton<DebugManager>
{
#if UNITY_EDITOR
    private const bool DEBUG = true;
#else
    private const bool DEBUG = false;
#endif
    [SerializeField] private DebugPlayerData _debugPlayerSave;
    [SerializeField] private bool _allowSkipDialogue;
    [SerializeField] private bool _allowSkipDay;

    public PlayerData DebugPlayerSave => DEBUG ? _debugPlayerSave?.PlayerData : null;
    public bool AllowSkipDialogue => DEBUG ? _allowSkipDialogue : false;
    public bool AllowSkipDay => DEBUG ? _allowSkipDay : false;
}
