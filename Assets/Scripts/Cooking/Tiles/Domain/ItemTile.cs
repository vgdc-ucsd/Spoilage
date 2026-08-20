public class ItemTile : ITile
{
    private Item _item;
    public ItemTile(Item item) { _item = item; }
    public bool Accepts(Placeable _) { return false; }
    public void Place(Placeable placeable) { }
    public Placeable Produces() { return _item; }
    public void Remove() { }
}
