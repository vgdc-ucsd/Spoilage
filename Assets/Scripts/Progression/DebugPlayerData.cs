using UnityEngine;

[CreateAssetMenu(fileName = "Save", menuName = "Progression/DebugSave")]
public class DebugPlayerData : ScriptableObject
{
    [SerializeField] private PlayerData _playerData;
    public PlayerData PlayerData => _playerData;
}
