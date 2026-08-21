using System.Collections.Generic;
using UnityEngine;

public class Day : ScriptableObject
{
    [SerializeField] private List<UpgradeID> _upgrades;
    public List<UpgradeID> Upgrades => _upgrades; 
}
