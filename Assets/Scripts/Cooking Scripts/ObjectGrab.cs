using UnityEngine;

public class ObjectGrab : MonoBehaviour
{
    public enum StationType { Counter, Appliance }
    public StationType type;
    private Vector3 _sidebarPosition;

    private void Awake() => _sidebarPosition = transform.position;

    public bool TryGrab()
    {
        if (LockLayout.IsLocked) return false;

        KitchenTile tile = GetTileAtPosition(transform.position);
        if (tile != null)
        {
            if (tile.GetTopObject() != gameObject) return false;
            tile.RemoveObject(gameObject);
        }

        transform.SetParent(null);
        return true;
    }

    public void UpdateDragPosition()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
        transform.position = new Vector3(mousePos.x, mousePos.y, (type == StationType.Appliance) ? -2f : -1f);
    }

    public void Drop()
    {
        KitchenTile tile = GetTileAtPosition(transform.position);
        string typeString = (type == StationType.Counter) ? "Counter" : "Appliance";

        if (tile != null && tile.CanPlaceObject(typeString, gameObject))
        {
            tile.PlaceObject(gameObject);
            transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, (type == StationType.Appliance) ? -2f : -1f);
        }
        else
        {
            transform.position = _sidebarPosition;
        }
        transform.SetParent(null);
    }

    private KitchenTile GetTileAtPosition(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(pos);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out KitchenTile tile)) return tile;
        }
        return null;
    }
}
