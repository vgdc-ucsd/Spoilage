public class Item : Placeable
{
    public override PlaceableUI UI => _ui;
    public ItemData Data => _data;
    
    private PlaceableUI _ui;
    private ItemData _data;

    public Item(ItemData data)
    {
        _data = data;
    }

    public void SetUI(PlaceableUI ui)
    {
        _ui = ui;
    }
}
