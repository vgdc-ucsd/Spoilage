using System.Collections.Generic;
using UnityEngine;

public class StoryGraph : ScriptableObject
{
    [SerializeField] private StoryGraphNode _root;

}

public class StoryGraphNode : ScriptableObject
{
    [SerializeField] private List<Day> _days;

}