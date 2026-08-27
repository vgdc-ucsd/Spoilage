using UnityEngine;

public abstract class Placeable
{
    public abstract PlaceableUI UI { get; }
    public virtual void Destroy()
    {
        Object.Destroy(UI.gameObject);
    }
}
