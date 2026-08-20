public interface ITile
{
    public abstract bool Accepts(Placeable placeable);
    public abstract void Place(Placeable placeable);
    public abstract Placeable Produces();
    public abstract void Remove();
}
