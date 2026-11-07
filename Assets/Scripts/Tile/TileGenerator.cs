
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    [Header("Tile Strengths")]
    [Range(0, 4)] public int topStrength;
    [Range(0, 4)] public int rightStrength;
    [Range(0, 4)] public int bottomStrength;
    [Range(0, 4)] public int leftStrength;

    [Header("Tick Settings")]
    [SerializeField] private GameObject tickPrefab;
    [SerializeField] private float tickSpacing = 0.8f;
    private GameBalanceConfig balanceConfig;

    private TickColor tileTickColor;
    private bool ticksGenerated;
    public TickColor TileTickColor
    {
        get { return tileTickColor; }
    }


    public void InitializeTickColor(
    TickColor color,
    GameBalanceConfig config)
    {
        tileTickColor = color;
        balanceConfig = config;

        if (ticksGenerated)
            return;

        RandomizeStrengths();
        GenerateTicks();

        ticksGenerated = true;
    }

    private void RandomizeStrengths()
    {
        int totalTicks;

        do
        {
            topStrength = Random.Range(
                balanceConfig.minimumSideStrength,
                balanceConfig.maximumSideStrength + 1
            );

            rightStrength = Random.Range(
                balanceConfig.minimumSideStrength,
                balanceConfig.maximumSideStrength + 1
            );

            bottomStrength = Random.Range(
                balanceConfig.minimumSideStrength,
                balanceConfig.maximumSideStrength + 1
            );

            leftStrength = Random.Range(
                balanceConfig.minimumSideStrength,
                balanceConfig.maximumSideStrength + 1
            );

            totalTicks =
                topStrength +
                rightStrength +
                bottomStrength +
                leftStrength;
        }
        while (
            totalTicks < balanceConfig.minimumTotalTicks ||
            totalTicks > balanceConfig.maximumTotalTicks
        );
    }

    private void GenerateTicks()
    {
        GenerateEdgeTicks(topStrength, Edge.Top);
        GenerateEdgeTicks(rightStrength, Edge.Right);
        GenerateEdgeTicks(bottomStrength, Edge.Bottom);
        GenerateEdgeTicks(leftStrength, Edge.Left);
    }

    private void GenerateEdgeTicks(int tickCount, Edge edge)
    {
        if (tickCount == 0)
            return;

        float startOffset =
            -((tickCount - 1) * tickSpacing) / 2f;

        for (int i = 0; i < tickCount; i++)
        {
            float offset = startOffset + (i * tickSpacing);

            Vector3 position;
            Quaternion rotation;

            switch (edge)
            {
                case Edge.Top:
                    position = new Vector3(offset, 0.51f, 0.40f);
                    rotation = Quaternion.identity;
                    break;

                case Edge.Bottom:
                    position = new Vector3(offset, 0.51f, -0.40f);
                    rotation = Quaternion.identity;
                    break;

                case Edge.Left:
                    position = new Vector3(-0.40f, 0.51f, offset);
                    rotation = Quaternion.Euler(0, 90, 0);
                    break;

                case Edge.Right:
                    position = new Vector3(0.40f, 0.51f, offset);
                    rotation = Quaternion.Euler(0, 90, 0);
                    break;

                default:
                    return;
            }

            GameObject tick = Instantiate(tickPrefab, transform);

            tick.transform.localPosition = position;
            tick.transform.localRotation = rotation;

            tick.name = $"{edge}_Tick_{i + 1}";

            TickView tickView = tick.GetComponent<TickView>();

            if (tickView == null)
            {
                Debug.LogError(
                    "Tick prefab needs TickView.cs",
                    tick
                );

                continue;
            }

            // Every tick on this tile receives the same color.
            tickView.Initialize(tileTickColor);
        }
    }

    private enum Edge
    {
        Top,
        Right,
        Bottom,
        Left
    }
}
