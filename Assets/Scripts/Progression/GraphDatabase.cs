using System.Collections.Generic;
using UnityEngine;

public class GraphDatabase<T> : ScriptableObject
{
    [SerializeField] private string _searchFolder;
    [SerializeField] private string _nodeTypeName;
    [SerializeField] private List<GraphNode<T>> _nodes;

    public string SearchFolder => _searchFolder;
    public string NodeTypeName => _nodeTypeName;
    public List<GraphNode<T>> Nodes => _nodes;

    public void SetNodes(List<GraphNode<T>> nodes)
    {
        _nodes = nodes;
    }
}
