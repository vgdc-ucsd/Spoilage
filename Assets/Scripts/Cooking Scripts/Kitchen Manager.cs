using UnityEngine;
using UnityEngine.InputSystem;

public class KitchenManager : MonoBehaviour
{
    public GameObject counterPrefab;
    public GameObject stovePrefab;

    private void Update()
    {
        // Check for left click
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryPlace();
        }
    }

    private void TryPlace()
    {
        // Raycast from camera to mouse position
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            // Try to find the KitchenTile component on the object we clicked
            KitchenTile tile = hit.collider.GetComponent<KitchenTile>();

            if (tile != null)
            {
                // EXAMPLE LOGIC: 
                // If the tile is empty, place a counter.
                // If it has a counter, place a stove.
                if (tile.CanPlaceObject("Counter"))
                {
                    tile.PlaceObject(counterPrefab, "Counter");
                }
                else if (tile.CanPlaceObject("Appliance"))
                {
                    tile.PlaceObject(stovePrefab, "Appliance");
                }
            }
        }
    }
}
