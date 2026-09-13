public class GiveItemTile : ITile
{
    private Item _item;
    private GiveItemTileUI _ui;

    public GiveItemTile(ItemData item, GiveItemTileUI ui) 
    {
        _item = new Item(item);
        _ui = ui;
        _item.SetUI(PlaceableUIFactory.Instance.Generate(item, ui.transform)); 
    }
    
    public void Remove()
    {
        _item = null;
        _ui.Hide();
        SetupManager.Instance.LockKitchenTiles(true);
        CustomerLineManager.Instance.ItemTaken();
    }

    public bool Accepts(Placeable _) { return false; }
    public void Place(Placeable placeable) { }
    public Placeable Produces() { return _item; }
}
