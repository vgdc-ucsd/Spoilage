using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Timeline", menuName = "Progression/Timeline")]
public class Timeline : ScriptableObject
{
    [SerializeField] private List<Day> _days;
    public List<Day> Days => _days;
}
