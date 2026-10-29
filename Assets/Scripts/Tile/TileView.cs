using System.Collections;
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

    [Header("Selection Jiggle")]
    [SerializeField] private float jiggleAngle = 5f;
    [SerializeField] private float jiggleSpeed = 18f;

    private GameDriver gameDriver;
    private MaterialPropertyBlock propertyBlock;
    private Coroutine jiggleCoroutine;
    private Quaternion originalLocalRotation;

    public void Initialize(PlayerId owner, GameDriver driver)
    {
        Owner = owner;
        gameDriver = driver;
        originalLocalRotation = transform.localRotation;

        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        UpdateAppearance();
    }

    public void SetOwner(PlayerId newOwner)
    {
        Owner = newOwner;
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        if (tileRenderer == null)
            tileRenderer = GetComponent<Renderer>();

        if (tileRenderer == null)
        {
            Debug.LogWarning("No tile Renderer found on " + name, this);
            return;
        }

        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        Color tileColor = Owner == PlayerId.Player1
            ? playerOneColor
            : playerTwoColor;

        tileRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", tileColor);
        propertyBlock.SetColor("_BaseColor", tileColor);
        tileRenderer.SetPropertyBlock(propertyBlock);
    }

    public void MarkAsPlaced(BoardSpace space)
    {
        SetSelected(false);
        IsPlaced = true;
        CurrentSpace = space;
    }

    private void OnMouseDown()
    {
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

    public void SetSelected(bool selected)
    {
        if (selected && !IsPlaced)
        {
            if (jiggleCoroutine == null)
                jiggleCoroutine = StartCoroutine(Jiggle());
        }
        else
        {
            StopJiggle();
        }
    }

    private IEnumerator Jiggle()
    {
        while (true)
        {
            float angle = Mathf.Sin(Time.time * jiggleSpeed)
                * jiggleAngle;

            transform.localRotation =
                originalLocalRotation *
                Quaternion.Euler(0f, angle, 0f);

            yield return null;
        }
    }

    private void StopJiggle()
    {
        if (jiggleCoroutine != null)
        {
            StopCoroutine(jiggleCoroutine);
            jiggleCoroutine = null;
        }

        transform.localRotation = originalLocalRotation;
    }

    private void OnDisable()
    {
        StopJiggle();
    }
}