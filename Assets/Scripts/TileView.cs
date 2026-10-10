
using UnityEngine;

public class TileView : MonoBehaviour
{
    public PlayerId Owner { get; private set; }

    public bool IsPlaced { get; private set; }

    public BoardSpace CurrentSpace { get; private set; }

    private GameDriver gameDriver;

    public void Initialize(PlayerId owner, GameDriver driver)
    {
        Owner = owner;
        gameDriver = driver;

        Debug.Log(
            "Tile initialized: " + name +
            " | Owner: " + Owner +
            " | Driver assigned: " + (gameDriver != null)
        );
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
        // Reserved for visual feedback, such as highlighting.
        // Ownership and legal-move checks are handled by GameDriver.
    }
}
