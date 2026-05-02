using UnityEngine;

public class KitchenGrid : MonoBehaviour
{
    [Header("Appliance Prefabs")]
    public GameObject stovePrefab;
    public GameObject potPrefab;
    public GameObject toasterPrefab;

    [Header("Grid Settings")]
    public GameObject tilePrefab;
    public int columns = 6;
    public int rows = 2;

    // Added a 2D array to keep track of tiles once spawned
    private KitchenTile[,] _tileGrid;

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

    [Header("Positioning")]
    public float paddingBottom = 1.5f; // Gap from the bottom edge (leave room for trash/ing)

    void Start()
    {
        // Initialize the array size based on rows/cols
        _tileGrid = new KitchenTile[columns, rows];

        GenerateGrid();

        // HARD-CODED SPAWNS: Triggered immediately after grid exists
        SpawnInitialAppliance(stovePrefab, 0, 0);   // Stove at Tile_0_0
        SpawnInitialAppliance(potPrefab, 1, 0);     // Pot at Tile_1_0
        SpawnInitialAppliance(toasterPrefab, 2, 0); // Toaster at Tile_2_0
    }
    void GenerateGrid()
    {
        if (tilePrefab == null) return;

        Camera cam = Camera.main;
        float screenHeight = 2f * cam.orthographicSize;
        float screenWidth = screenHeight * cam.aspect;

        // 1. Calculate usable width based on your percentage
        float playableWidth = screenWidth * gridAreaWidthPercentage;
        float finalTileSize = playableWidth / columns;

        float totalGridWidth = finalTileSize * columns;
        float totalGridHeight = finalTileSize * rows;

        // 2. NEW POSITIONING LOGIC:
        // Start from the Right Edge and move left by the total width + padding
        float rightEdge = cam.transform.position.x + (screenWidth / 2);
        float startX = rightEdge - totalGridWidth;

        // Start from the Bottom Edge and move up by the total height + padding
        float bottomEdge = cam.transform.position.y - (screenHeight / 2);
        float startY = bottomEdge + totalGridHeight + paddingBottom;

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
                newTile.transform.localScale = new Vector3(finalTileSize * tileSpacing, finalTileSize * tileSpacing, 1);
                newTile.name = $"Tile_{x}_{y}";

                // Save the KitchenTile component into our array for later use
                if (newTile.TryGetComponent(out KitchenTile tileScript))
                {
                    _tileGrid[x, y] = tileScript;
                }

                AddBorderToTile(newTile);
            }
        }
    }
    void SpawnInitialAppliance(GameObject prefab, int x, int y)
    {
        if (prefab == null) return;

        KitchenTile targetTile = _tileGrid[x, y];
        if (targetTile != null)
        {
            GameObject obj = Instantiate(prefab, targetTile.transform.position, Quaternion.identity);

            // IMPORTANT: Link the appliance to this tile immediately
            if (obj.TryGetComponent(out ObjectGrab station))
            {
                station.currentTile = targetTile;
            }

            // Register in tile inventory and snap to -2f
            targetTile.PlaceObject(obj);
            obj.transform.SetParent(null);
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
