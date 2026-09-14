public class ReceiveItemTile : ITile
{
    public bool Accepts(Placeable placeable) 
    { 
        return placeable is Item;
    }

    public void Place(Placeable placeable)
    {
        
    }
    
    public void Remove() { }
    public Placeable Produces() { return null; }
}
