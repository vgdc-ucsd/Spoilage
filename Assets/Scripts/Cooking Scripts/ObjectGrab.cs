using UnityEngine;

public class ObjectGrab : MonoBehaviour
{
    public enum StationType { Counter, Appliance }
    public StationType type;

    private Vector3 _sidebarPosition;

    private void Awake() => _sidebarPosition = transform.position;

    public bool TryGrab()
    {
        // Find tile at current position
        KitchenTile tile = GetTileAtPosition(transform.position);

        if (tile != null)
        {
            // If it's not the top object, we can't grab it
            if (tile.GetTopObject() != gameObject) return false;

            tile.RemoveObject(gameObject);
        }

        if (type == StationType.Appliance) transform.SetParent(null);
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

            if (type == StationType.Appliance && tile.objectsOnTile.Count > 0)
            {
                GameObject counter = tile.objectsOnTile[0];
                transform.position = counter.transform.position;
                transform.SetParent(counter.transform);
            }
            else
            {
                transform.position = tile.transform.position;
            }
        }
        else
        {
            transform.position = _sidebarPosition;
            transform.SetParent(null);
        }
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
