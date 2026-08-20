using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropManager : Singleton<DragAndDropManager>
{
    [SerializeField] private Transform _dragLayer;
    private Stack<TileUI> _hoveredTiles = new Stack<TileUI>();
    private TileUI _selectedTile;

    public void Hover(TileUI tile)
    {
        _hoveredTiles.Push(tile);
    }

    public void Unhover()
    {
        _hoveredTiles.Pop();
    }

    public void BeginDrag(TileUI tileUI)
    {
        if (tileUI.Tile.Produces() != null) 
        {
            PlaceableUI ui = tileUI.Tile.Produces().UI; 
            ui.SetDrag(true);
            ui.transform.SetParent(_dragLayer);
            _selectedTile = tileUI;
        }
        else _selectedTile = null;
    }

    public void Drag(PointerEventData eventData)
    {
        if (_selectedTile == null) return;
        PlaceableUI ui = _selectedTile.Tile.Produces().UI;
        ui.transform.position = eventData.position;
    }

    public void EndDrag()
    {
        if (_selectedTile == null) return;
        
        if (_hoveredTiles.Count == 0) 
        {
            Unselect();
            return;
        }
        
        TileUI destination = _hoveredTiles.Peek();
        Placeable placeable = _selectedTile.Tile.Produces();

        if (destination.Tile.Accepts(placeable))
        {
            destination.Tile.Place(placeable);
            placeable.UI.SetDrag(false);
            _selectedTile.Tile.Remove();
        }
        else
        {   
            Unselect();
        }
    }

    public void Click()
    {
        
    }

    private void Unselect()
    {
        if (_selectedTile == null) return;
        PlaceableUI ui = _selectedTile.Tile.Produces().UI;
        ui.SetDrag(false);
        ui.transform.SetParent(_selectedTile.transform);
        ui.transform.localPosition = Vector3.zero;
    }
}
