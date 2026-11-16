
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    [Header("Tile")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameBalanceConfig balanceConfig;

    [Header("Layout")]
    [SerializeField] private float tileSpacing = 1.8f;
    [SerializeField] private float playerOneZ = -5.5f;
    [SerializeField] private float playerTwoZ = 5.5f;

    private void Start()
    {
        SpawnPlayerTiles();
    }

    private void SpawnPlayerTiles()
    {
        if (balanceConfig == null)
        {
            Debug.LogError("[TileSpawner] GameBalanceConfig is not assigned!", this);
            DisableGameDriver();
            return;
        }

        if (!balanceConfig.ValidateConfig(out string errorMessage))
        {
            Debug.LogError(
                $"[TileSpawner] GameBalanceConfig validation failed: {errorMessage}. Aborting tile spawning.",
                this
            );
            DisableGameDriver();
            return;
        }

        if (tilePrefab == null)
        {
            Debug.LogError("[TileSpawner] tilePrefab is not assigned!", this);
            DisableGameDriver();
            return;
        }

        if (tilePrefab.GetComponent<TileGenerator>() == null)
        {
            Debug.LogError("[TileSpawner] tilePrefab is missing TileGenerator component!", this);
            DisableGameDriver();
            return;
        }

        if (tilePrefab.GetComponent<TileView>() == null)
        {
            Debug.LogError("[TileSpawner] tilePrefab is missing TileView component!", this);
            DisableGameDriver();
            return;
        }

        List<GameObject> currentSpawnedTiles = new List<GameObject>();

        if (!SpawnTileRow(playerOneZ, PlayerId.Player1, "Player1", currentSpawnedTiles))
        {
            CleanupSpawnedTiles(currentSpawnedTiles);
            DisableGameDriver();
            return;
        }

        if (!SpawnTileRow(playerTwoZ, PlayerId.Player2, "Player2", currentSpawnedTiles))
        {
            CleanupSpawnedTiles(currentSpawnedTiles);
            DisableGameDriver();
            return;
        }
    }

    private bool SpawnTileRow(
        float zPosition,
        PlayerId owner,
        string playerName,
        List<GameObject> currentSpawnedTiles)
    {
        float startX =
            -((balanceConfig.tilesPerPlayer - 1) * tileSpacing) / 2f;

        // Give each player a balanced distribution of colors.
        List<TickColor> colors = CreateBalancedColors();

        for (int i = 0; i < balanceConfig.tilesPerPlayer; i++)
        {
            float xPosition = startX + i * tileSpacing;

            Vector3 position = new Vector3(
                xPosition,
                0.5f,
                zPosition
            );

            GameObject tile = Instantiate(
                tilePrefab,
                position,
                Quaternion.identity,
                transform
            );

            tile.name = $"{playerName}_Tile_{i}";
            currentSpawnedTiles.Add(tile);

            TileGenerator tileGenerator =
                tile.GetComponent<TileGenerator>();

            if (tileGenerator == null)
            {
                Debug.LogError(
                    "[TileSpawner] Tile instance missing TileGenerator.",
                    tile
                );
                return false;
            }

            if (!tileGenerator.InitializeTickColor(colors[i], balanceConfig))
            {
                Debug.LogError(
                    $"[TileSpawner] Tile initialization failed for {tile.name}. Aborting tile spawning.",
                    tile
                );
                return false;
            }

            TileView tileView = tile.GetComponent<TileView>();

            if (tileView == null)
            {
                Debug.LogError(
                    "[TileSpawner] Tile instance missing TileView.",
                    tile
                );
                return false;
            }

            // The GameDriver connects the tiles after scene setup.
            tileView.Initialize(owner, null);
        }

        return true;
    }

    private void CleanupSpawnedTiles(List<GameObject> spawnedTiles)
    {
        if (spawnedTiles == null)
            return;

        foreach (GameObject tile in spawnedTiles)
        {
            if (tile != null)
            {
                Destroy(tile);
            }
        }

        spawnedTiles.Clear();
    }

    private void DisableGameDriver()
    {
        if (GameDriver.Instance != null)
        {
            GameDriver.Instance.enabled = false;
        }
    }

    private List<TickColor> CreateBalancedColors()
    {
        List<TickColor> colors = new List<TickColor>();

        // Rotate the starting color so the distribution can vary.
        int startingColor = Random.Range(0, 3);

        for (int i = 0; i < balanceConfig.tilesPerPlayer; i++)
        {
            TickColor color = (TickColor)(
                (i + startingColor) % 3
            );

            colors.Add(color);
        }

        // Shuffle the colors so they aren't grouped by color.
        for (int i = colors.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            TickColor temp = colors[i];
            colors[i] = colors[j];
            colors[j] = temp;
        }

        return colors;
    }
}
