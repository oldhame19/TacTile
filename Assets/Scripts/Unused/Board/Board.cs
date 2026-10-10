using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    private List<Space> spaces = new List<Space>();

    public IReadOnlyList<Space> Spaces => spaces;

    private void Start()
    {
        Generate5x5Board();
        ValidateBoard();
    }

    public void Generate5x5Board()
    {
        spaces.Clear();

        const int width = 5;
        const int height = 5;

        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                int id = row * width + column + 1;

                Space space = new Space
                {
                    id = id,
                    displayName = $"Space {id}",
                    position = new Vector2(column, -row),
                    spaceKind = SpaceKind.Normal
                };

                if (column > 0)
                    space.neighborIds.Add(id - 1);

                if (column < width - 1)
                    space.neighborIds.Add(id + 1);

                if (row > 0)
                    space.neighborIds.Add(id - width);

                if (row < height - 1)
                    space.neighborIds.Add(id + width);

                spaces.Add(space);
            }
        }
    }

    public List<Space> GetNeighbors(int spaceId)
    {
        Space space = spaces.Find(s => s.id == spaceId);

        if (space == null)
        {
            Debug.LogWarning($"Space with ID {spaceId} was not found.");
            return new List<Space>();
        }

        List<Space> neighbors = new List<Space>();

        foreach (int neighborId in space.neighborIds)
        {
            Space neighbor = spaces.Find(s => s.id == neighborId);

            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    public int SpaceCount()
    {
        return spaces.Count;
    }

    public void ValidateBoard()
    {
        Debug.Log($"Board contains {spaces.Count} spaces.");

        bool adjacencyIsSymmetric = true;

        foreach (Space space in spaces)
        {
            foreach (int neighborId in space.neighborIds)
            {
                Space neighbor = spaces.Find(s => s.id == neighborId);

                if (neighbor == null)
                {
                    Debug.LogError(
                        $"Space {space.id} references missing neighbor {neighborId}."
                    );

                    adjacencyIsSymmetric = false;
                    continue;
                }

                if (!neighbor.neighborIds.Contains(space.id))
                {
                    Debug.LogError(
                        $"Adjacency error: Space {space.id} lists Space {neighborId}, " +
                        $"but Space {neighborId} does not list Space {space.id}."
                    );

                    adjacencyIsSymmetric = false;
                }
            }
        }

        Debug.Log($"Adjacency symmetric: {adjacencyIsSymmetric}");
    }
}