using UnityEngine;

public class KitchenGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject tilePrefab;
    public int columns = 6;
    public int rows = 2;

    [Header("Tile Size & Spacing")]
    public float manualTileSize = 1.75f;

    [Range(0.1f, 1.0f)]
    public float tileSpacing = 1.0f;

    [Header("Positioning")]
    public float rightPadding = 0f;
    public float bottomPadding = 2.5f;

    [Header("Border Settings")]
    public Color borderColor = Color.black;
    [Range(0.01f, 0.1f)]
    public float borderThickness = 0.03f;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        if (tilePrefab == null) return;

        Camera cam = Camera.main;
        float screenHeight = 2f * cam.orthographicSize;
        float screenWidth = screenHeight * cam.aspect;

        // 1. Calculate dimensions
        float totalGridWidth = manualTileSize * columns;

        // 2. Position math for Bottom-Right alignment
        // Removing rightPadding and aligning strictly to (Center + HalfWidth)
        float startX = (cam.transform.position.x + screenWidth / 2) - totalGridWidth - rightPadding;

        // Increased bottomPadding lifts the 'base' of the grid higher from the screen bottom
        float startY = (cam.transform.position.y - screenHeight / 2) + (manualTileSize * rows) + bottomPadding;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 spawnPos = new Vector3(
                    startX + (x * manualTileSize) + (manualTileSize / 2),
                    startY - (y * manualTileSize) - (manualTileSize / 2),
                    0
                );

                GameObject newTile = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);

                newTile.transform.localScale = new Vector3(manualTileSize * tileSpacing, manualTileSize * tileSpacing, 0);
                newTile.name = $"Tile_{x}_{y}";

                AddBorderToTile(newTile);
            }
        }
    }

    void AddBorderToTile(GameObject tile)
    {
        LineRenderer line = tile.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.widthMultiplier = borderThickness;
        line.positionCount = 5;
        line.loop = true;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = line.endColor = borderColor;
        line.sortingOrder = 0;

        float s = 0.5f;
        line.SetPosition(0, new Vector3(-s, -s, 0));
        line.SetPosition(1, new Vector3(s, -s, 0));
        line.SetPosition(2, new Vector3(s, s, 0));
        line.SetPosition(3, new Vector3(-s, s, 0));
        line.SetPosition(4, new Vector3(-s, -s, 0));
    }
}
