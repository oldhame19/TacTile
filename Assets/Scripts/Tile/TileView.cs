
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

    [Header("Boost Highlight")]
    [SerializeField] private float boostPulseSpeed = 2.5f;
    [SerializeField] private float playerOneHighlightIntensity = 0.65f;
    [SerializeField] private float playerTwoHighlightIntensity = 0.25f;

    private GameDriver gameDriver;
    private MaterialPropertyBlock propertyBlock;
    private Coroutine jiggleCoroutine;
    private Coroutine boostCoroutine;
    private Quaternion originalLocalRotation;

    private Color boostHighlightColor;
    private bool isBoosted;

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

        if (isBoosted)
        {
            float intensity = Owner == PlayerId.Player1
                ? playerOneHighlightIntensity
                : playerTwoHighlightIntensity;

            tileColor = Color.Lerp(
                tileColor,
                boostHighlightColor,
                intensity
            );
        }

        tileRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", tileColor);
        propertyBlock.SetColor("_BaseColor", tileColor);
        tileRenderer.SetPropertyBlock(propertyBlock);
    }

    public void SetBoostHighlight(bool boosted, Color highlightColor)
    {
        isBoosted = boosted;
        boostHighlightColor = highlightColor;

        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
            boostCoroutine = null;
        }

        if (isBoosted)
        {
            boostCoroutine = StartCoroutine(BoostPulse());
        }
        else
        {
            UpdateAppearance();
        }
    }

    private IEnumerator BoostPulse()
    {
        while (true)
        {
            float pulse = (Mathf.Sin(Time.time * boostPulseSpeed) + 1f) / 2f;

            float maxIntensity = Owner == PlayerId.Player1
                ? playerOneHighlightIntensity
                : playerTwoHighlightIntensity;

            float intensity = pulse * maxIntensity;

            Color baseColor = Owner == PlayerId.Player1
                ? playerOneColor
                : playerTwoColor;

            Color displayColor = Color.Lerp(
                baseColor,
                boostHighlightColor,
                intensity
            );

            tileRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_Color", displayColor);
            propertyBlock.SetColor("_BaseColor", displayColor);
            tileRenderer.SetPropertyBlock(propertyBlock);

            yield return null;
        }
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

        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
            boostCoroutine = null;
        }
    }
}
