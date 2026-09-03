using UnityEngine;

public class GraphNode<T> : ScriptableObject
{
    [SerializeField] private int _day;
    [SerializeField] private T _data;
    [SerializeField] private GraphNode<T> _noBranch;
    [SerializeField] private Branch<T> _branch;
    [SerializeField] private int _id;
    
    public int Day => _day;
    public T Data => _data;
    public GraphNode<T> Next => _noBranch;
    public Branch<T> Branch => _branch;
    public int ID => _id;

    public void SetID(int id)
    {
        _id = id;
    }

    public GraphNode<T> Advance(int day)
    {
        if (day < _day) return this;
        if (_noBranch != null) return _noBranch;
        return _branch.Next();
    }
}
    