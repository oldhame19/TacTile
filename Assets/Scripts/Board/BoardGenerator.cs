
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    [Header("Board Settings")]
    [SerializeField] private GameObject boardSquarePrefab;
    [SerializeField] private int boardSize = 5;
    [SerializeField] private float squareSpacing = 2f;

    [Header("Boost Settings")]
    [SerializeField] private int boostsPerColor = 2;

    private void Start()
    {
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        float boardOffset =
            (boardSize - 1) * squareSpacing / 2f;

        List<BoardSpace> spaces = new List<BoardSpace>();

        for (int row = 0; row < boardSize; row++)
        {
            for (int column = 0; column < boardSize; column++)
            {
                float x = column * squareSpacing - boardOffset;
                float z = row * squareSpacing - boardOffset;

                Vector3 position = new Vector3(x, 0.3f, z);

                GameObject square = Instantiate(
                    boardSquarePrefab,
                    position,
                    Quaternion.identity,
                    transform
                );

                square.name = $"Square_{row}_{column}";

                BoardSpace boardSpace =
                    square.GetComponent<BoardSpace>();

                if (boardSpace == null)
                {
                    Debug.LogError(
                        "Board square prefab needs BoardSpace.cs",
                        square
                    );

                    Destroy(square);
                    continue;
                }

                boardSpace.Initialize(row, column);
                spaces.Add(boardSpace);
            }
        }

        AssignBoostSpaces(spaces);
    }

    private void AssignBoostSpaces(List<BoardSpace> spaces)
    {
        if (boardSize < 3)
        {
            Debug.LogError(
                "Board must be at least 3x3 for a center boost."
            );

            return;
        }

        int centerRow = boardSize / 2;
        int centerColumn = boardSize / 2;

        BoardSpace centerSpace = spaces.Find(space =>
            space.Row == centerRow &&
            space.Column == centerColumn
        );

        if (centerSpace == null)
        {
            Debug.LogError("Could not find the center board space.");
            return;
        }

        // The center is always neutral and purple.
        centerSpace.SetBoost(BoostType.Neutral);

        // Exclude the center from the random boost locations.
        List<BoardSpace> availableSpaces =
            spaces.FindAll(space => space != centerSpace);

        // Shuffle the available spaces.
        for (int i = availableSpaces.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            BoardSpace temp = availableSpaces[i];
            availableSpaces[i] = availableSpaces[j];
            availableSpaces[j] = temp;
        }

        BoostType[] boostTypes =
        {
            BoostType.Red,
            BoostType.LightBlue,
            BoostType.Yellow
        };

        int requiredSpaces = boostsPerColor * boostTypes.Length;

        if (boostsPerColor < 0 ||
            availableSpaces.Count < requiredSpaces)
        {
            Debug.LogError(
                "Not enough board spaces for the requested boosts."
            );

            return;
        }

        int index = 0;

        foreach (BoostType boostType in boostTypes)
        {
            for (int i = 0; i < boostsPerColor; i++)
            {
                availableSpaces[index].SetBoost(boostType);
                index++;
            }
        }

        Debug.Log(
            "Boost spaces generated. Neutral: 1, Colored: " +
            requiredSpaces
        );
    }
}
