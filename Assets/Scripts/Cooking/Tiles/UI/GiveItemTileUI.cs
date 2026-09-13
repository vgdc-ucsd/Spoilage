public class GiveItemTileUI : TileUI
{
    public void Show(ItemData item)
    {
        gameObject.SetActive(true);
        Tile = new GiveItemTile(item, this);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
