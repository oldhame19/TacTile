using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    [Header("Board Settings")]
    [SerializeField] private GameObject boardSquarePrefab;

    [SerializeField] private int boardSize = 4;
    [SerializeField] private float squareSpacing = 2f;

    private void Start()
    {
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        float boardOffset = (boardSize - 1) * squareSpacing / 2f;

        for (int row = 0; row < boardSize; row++)
        {
            for (int column = 0; column < boardSize; column++)
            {
                float x = (column * squareSpacing) - boardOffset;
                float z = (row * squareSpacing) - boardOffset;

                Vector3 position = new Vector3(x, 0.3f, z);

                GameObject square = Instantiate(
                    boardSquarePrefab,
                    position,
                    Quaternion.identity,
                    transform
                );

                square.name = $"Square_{row}_{column}";
            }
        }
    }
}