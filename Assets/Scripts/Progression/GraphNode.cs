using UnityEngine;

public class GraphNode<T> : ScriptableObject
{
    [SerializeField] private int _day;
    [SerializeField] private T _data;
    [SerializeField] private Branch<GraphNode<T>> _branch;
    
    public int Day => _day;
    public T Data => _data;
    public Branch<GraphNode<T>> branch => _branch;
}
    