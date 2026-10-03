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
        SpawnTileRow(playerOneZ, "Player1");
        SpawnTileRow(playerTwoZ, "Player2");
    }

    private void SpawnTileRow(float zPosition, string playerName)
    {
        float startX = -((tilesPerPlayer - 1) * tileSpacing) / 2f;

        for (int i = 0; i < tilesPerPlayer; i++)
        {
            float xPosition = startX + (i * tileSpacing);

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
        }
    }
}