using UnityEngine;

public abstract class Branch<T> : ScriptableObject
{
    public abstract GraphNode<T> Next();
}
