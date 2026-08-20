public class PlatingTile : ITile
{
    private Food _food;

    public bool Accepts(Placeable placeable)
    {
        return _food == null && placeable is Food;
    }
    
    public void Place(Placeable placeable)
    {
        _food = placeable as Food;
    }

    public Placeable Produces()
    {
        return _food;
    }
    
    public void Remove()
    {
        _food = null;
    }
}
