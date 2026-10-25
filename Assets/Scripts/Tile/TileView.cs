
using UnityEngine;

public class TileView : MonoBehaviour
{
    public PlayerId Owner { get; private set; }

    public bool IsPlaced { get; private set; }

    public BoardSpace CurrentSpace { get; private set; }

    [Header("Tile Appearance")]
    [SerializeField] private Renderer tileRenderer;

    [SerializeField] private Color playerOneColor = Color.white;
    [SerializeField] private Color playerTwoColor = Color.black;

    private GameDriver gameDriver;

    private MaterialPropertyBlock propertyBlock;

    public void Initialize(PlayerId owner, GameDriver driver)
    {
        Owner = owner;
        gameDriver = driver;

        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        UpdateAppearance();

        Debug.Log(
            "Tile initialized: " + name +
            " | Owner: " + Owner +
            " | Driver assigned: " + (gameDriver != null)
        );
    }

    public void SetOwner(PlayerId newOwner)
    {
        Owner = newOwner;
        UpdateAppearance();

        Debug.Log(
            name + " ownership changed to " + Owner
        );
    }

    private void UpdateAppearance()
    {
        if (tileRenderer == null)
        {
            tileRenderer = GetComponent<Renderer>();
        }

        if (tileRenderer == null)
        {
            Debug.LogWarning(
                "No tile Renderer found on " + name +
                ". Assign the tile's body Renderer in the Inspector.",
                this
            );

            return;
        }

        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        Color tileColor = Owner == PlayerId.Player1
            ? playerOneColor
            : playerTwoColor;

        tileRenderer.GetPropertyBlock(propertyBlock);

        // Support both common Built-in and URP/HDRP color properties.
        propertyBlock.SetColor("_Color", tileColor);
        propertyBlock.SetColor("_BaseColor", tileColor);

        tileRenderer.SetPropertyBlock(propertyBlock);
    }

    public void MarkAsPlaced(BoardSpace space)
    {
        IsPlaced = true;
        CurrentSpace = space;
    }

    private void OnMouseDown()
    {
        Debug.Log("Tile clicked: " + name);

        if (gameDriver == null)
        {
            Debug.LogError("Tile has no GameDriver reference.");
            return;
        }

        gameDriver.TrySelectTile(this);
    }

    public void SetSelectable(bool selectable)
    {
        // Reserved for visual feedback.
    }
}
