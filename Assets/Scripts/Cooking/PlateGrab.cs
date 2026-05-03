using UnityEngine;

public class PlateGrab : MonoBehaviour
{
    private Vector3 _sidebarPosition;

    // We use -3f to ensure it is in front of everything else at the start
    private void Awake() => _sidebarPosition = transform.position;

    public bool TryGrab()
    {
        // 1. No LockLayout check here.
        // 2. We check for a tile just like ObjectGrab does
        KitchenTile tile = GetTileAtPosition(transform.position);
        if (tile != null)
        {
            // We'll skip the GetTopObject check for now to ensure it always grabs
            tile.RemoveObject(gameObject);
        }

        transform.SetParent(null);
        return true;
    }

    public void UpdateDragPosition()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
        // Mirroring your ObjectGrab logic: Appliances are -2f, so we'll use -3f
        transform.position = new Vector3(mousePos.x, mousePos.y, -3f);
    }

    public void Drop()
    {
        // Mirroring your ObjectGrab Drop logic
        KitchenTile tile = GetTileAtPosition(transform.position);

        if (tile != null && tile.CanPlaceObject("Food", gameObject))
        {
            tile.PlaceObject(gameObject);
            transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -1.5f);
        }
        else
        {
            transform.position = _sidebarPosition;
        }
        transform.SetParent(null);
    }

    private KitchenTile GetTileAtPosition(Vector2 pos)
    {
        // Exact copy-paste from your ObjectGrab
        Collider2D[] hits = Physics2D.OverlapPointAll(pos);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out KitchenTile tile)) return tile;
        }
        return null;
    }
}
