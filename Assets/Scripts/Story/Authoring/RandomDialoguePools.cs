using UnityEngine;

/// <summary>
/// Prefixes for the random-customer dialogue pools. Each
/// field holds the base sequence name the registry queries.
/// </summary>
[CreateAssetMenu(fileName = "RandomDialoguePools", menuName = "Story/Random Dialogue Pools", order = 7)]
public class RandomDialoguePools : ScriptableObject
{
    public string introPrefix;
    public string refusePrefix;
    public string serveSuccessPrefix;
    public string serveFailPrefix;
}
