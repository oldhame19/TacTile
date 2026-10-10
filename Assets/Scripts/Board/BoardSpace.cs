using UnityEngine;

public class BoardSpace : MonoBehaviour
{
    public int Row { get; private set; }
    public int Column { get; private set; }

    public TileView OccupyingTile { get; private set; }

    public bool IsOccupied
    {
        get { return OccupyingTile != null; }
    }
    
    private void OnMouseDown()
    {
        Debug.Log("Board space clicked: " + name);

        if (GameDriver.Instance == null)
        {
            Debug.LogError("GameDriver.Instance is null.");
            return;
        }

        GameDriver.Instance.TryPlaceTile(this);
    }
    public void Initialize(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public bool TryPlaceTile(TileView tile)
    {
        if (tile == null || IsOccupied)
        {
            return false;
        }

        OccupyingTile = tile;

        tile.MarkAsPlaced(this);

        // Move the tile from its original row onto the board.
        tile.transform.SetParent(transform);
        tile.transform.position = transform.position + Vector3.up * 0.25f;

        // Keep the tile aligned with the board square.
        tile.transform.rotation = transform.rotation;

        return true;
    }
}
