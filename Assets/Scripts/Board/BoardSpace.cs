
using UnityEngine;

public enum BoostType
{
    Neutral,
    Red,
    LightBlue,
    Yellow
}

public class BoardSpace : MonoBehaviour
{
    public int Row { get; private set; }
    public int Column { get; private set; }

    public TileView OccupyingTile { get; private set; }

    public BoostType Boost { get; private set; } = BoostType.Neutral;

    public bool IsBoostSpace { get; private set; }

    [Header("Boost Appearance")]
    [SerializeField] private Renderer spaceRenderer;

    [SerializeField]
    private Color neutralColor =
        new Color(0.6f, 0.2f, 0.8f);

    [SerializeField] private Color redColor = Color.red;

    [SerializeField]
    private Color lightBlueColor =
        new Color(0.4f, 0.8f, 1f);

    [SerializeField] private Color yellowColor = Color.yellow;

    private MaterialPropertyBlock propertyBlock;

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

    public void SetBoost(BoostType boostType)
    {
        Boost = boostType;
        IsBoostSpace = true;

        UpdateBoostAppearance();
    }

    private void UpdateBoostAppearance()
    {
        if (spaceRenderer == null)
        {
            spaceRenderer = GetComponentInChildren<Renderer>();
        }

        if (spaceRenderer == null)
        {
            Debug.LogWarning(
                "No Renderer found for board space: " + name,
                this
            );

            return;
        }

        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        Color boostColor;

        switch (Boost)
        {
            case BoostType.Neutral:
                boostColor = neutralColor;
                break;

            case BoostType.Red:
                boostColor = redColor;
                break;

            case BoostType.LightBlue:
                boostColor = lightBlueColor;
                break;

            case BoostType.Yellow:
                boostColor = yellowColor;
                break;

            default:
                boostColor = Color.white;
                break;
        }

        spaceRenderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetColor("_Color", boostColor);
        propertyBlock.SetColor("_BaseColor", boostColor);

        spaceRenderer.SetPropertyBlock(propertyBlock);
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
