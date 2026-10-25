
using UnityEngine;

public enum TickColor
{
    Red,
    LightBlue,
    Yellow
}

public class TickView : MonoBehaviour
{
    public TickColor ColorType { get; private set; }

    [Header("Tick Appearance")]
    [SerializeField] private Renderer tickRenderer;

    [SerializeField] private Color redColor = Color.red;
    [SerializeField]
    private Color lightBlueColor =
        new Color(0.4f, 0.8f, 1f);

    [SerializeField] private Color yellowColor = Color.yellow;

    private MaterialPropertyBlock propertyBlock;

    public void Initialize(TickColor colorType)
    {
        ColorType = colorType;
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        if (tickRenderer == null)
        {
            tickRenderer = GetComponentInChildren<Renderer>();
        }

        if (tickRenderer == null)
        {
            Debug.LogWarning(
                "No Renderer found for tick: " + name,
                this
            );

            return;
        }

        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        Color selectedColor;

        switch (ColorType)
        {
            case TickColor.Red:
                selectedColor = redColor;
                break;

            case TickColor.LightBlue:
                selectedColor = lightBlueColor;
                break;

            case TickColor.Yellow:
                selectedColor = yellowColor;
                break;

            default:
                selectedColor = Color.white;
                break;
        }

        tickRenderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetColor("_Color", selectedColor);
        propertyBlock.SetColor("_BaseColor", selectedColor);

        tickRenderer.SetPropertyBlock(propertyBlock);
    }
}
