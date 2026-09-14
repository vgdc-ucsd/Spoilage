public class ReceiveItemTileUI : TileUI
{
    void Start()
    {
        Tile = new ReceiveItemTile();
        gameObject.SetActive(false);
    }
}
