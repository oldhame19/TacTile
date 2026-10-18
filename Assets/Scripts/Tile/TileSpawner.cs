
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    [Header("Tile")]
    [SerializeField] private GameObject tilePrefab;

    [Header("Player Settings")]
    [SerializeField] private int tilesPerPlayer = 8;

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
        SpawnTileRow(playerOneZ, PlayerId.Player1, "Player1");
        SpawnTileRow(playerTwoZ, PlayerId.Player2, "Player2");
    }

    private void SpawnTileRow(
        float zPosition,
        PlayerId owner,
        string playerName)
    {
        float startX =
            -((tilesPerPlayer - 1) * tileSpacing) / 2f;

        // Give each player a balanced distribution of colors.
        List<TickColor> colors = CreateBalancedColors();

        for (int i = 0; i < tilesPerPlayer; i++)
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

            // Assign one color to all ticks on this tile.
            TileGenerator tileGenerator =
                tile.GetComponent<TileGenerator>();

            if (tileGenerator == null)
            {
                Debug.LogError(
                    "Tile prefab needs TileGenerator.cs",
                    tile
                );
            }
            else
            {
                tileGenerator.InitializeTickColor(colors[i]);
            }

            TileView tileView = tile.GetComponent<TileView>();

            if (tileView == null)
            {
                Debug.LogError(
                    "Tile prefab needs TileView.cs",
                    tile
                );

                continue;
            }

            // The GameDriver connects the tiles after scene setup.
            tileView.Initialize(owner, null);
        }
    }

    private List<TickColor> CreateBalancedColors()
    {
        List<TickColor> colors = new List<TickColor>();

        // Rotate the starting color so the distribution can vary.
        int startingColor = Random.Range(0, 3);

        for (int i = 0; i < tilesPerPlayer; i++)
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
