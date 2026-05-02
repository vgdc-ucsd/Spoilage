using UnityEngine;

public class KitchenGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject tilePrefab;
    public int columns;
    public int rows;

    [Range(0.0f, 1.0f)]
    public float tileSpacing = 0.95f;

    [Header("Screen Layout Settings")]
    [Range(0.1f, 1.0f)]
    public float gridAreaWidthPercentage = 0.9f;
    [Range(0.1f, 1.0f)]
    public float gridAreaHeightPercentage = 0.8f;

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

        // 1. Calculate the total width we are allowed to use
        float playableWidth = screenWidth * gridAreaWidthPercentage;

        // 2. Determine tile size based ONLY on the width to ensure 6 fit across
        // This prevents the "only 4 tiles showing" issue
        float finalTileSize = playableWidth / columns;

        // 3. Calculate total grid dimensions for centering
        float totalGridWidth = finalTileSize * columns;
        float totalGridHeight = finalTileSize * rows;

        // 4. Center the grid
        // startX: Pushes it to the left but keeps it centered within the 90%
        float startX = cam.transform.position.x - (screenWidth / 2) + (screenWidth * (1 - gridAreaWidthPercentage) / 2);

        // startY: Centers the 2 rows vertically on the screen
        float startY = cam.transform.position.y + (totalGridHeight / 2);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 spawnPos = new Vector3(
                    startX + (x * finalTileSize) + (finalTileSize / 2),
                    startY - (y * finalTileSize) - (finalTileSize / 2),
                    0
                );

                GameObject newTile = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);

                // Use finalTileSize for BOTH X and Y to keep them square
                newTile.transform.localScale = new Vector3(finalTileSize * tileSpacing, finalTileSize * tileSpacing, 1);

                newTile.name = $"Tile_{x}_{y}";
            }
        }
    }
}
