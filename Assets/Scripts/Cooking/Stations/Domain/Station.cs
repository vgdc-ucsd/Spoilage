public enum FoodState
{
    Preparing,
    Prepared
}

public abstract class Station : Placeable, ITile
{
    public StationData Data { get; protected set; }
    public FoodState FoodState { get; protected set; }

    public bool Accepts(Placeable placeable) { return placeable is Food; }
    public abstract void Place(Placeable placeable);
    public abstract Placeable Produces();
    public abstract void Remove();
}
