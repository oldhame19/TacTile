
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


    public bool InitializeTickColor(
        TickColor color,
        GameBalanceConfig config)
    {
        if (config == null)
        {
            Debug.LogError(
                $"[TileGenerator] GameBalanceConfig passed to {name} is null.",
                this
            );
            return false;
        }

        tileTickColor = color;
        balanceConfig = config;

        if (ticksGenerated)
            return true;

        if (tickPrefab == null)
        {
            Debug.LogError(
                $"[TileGenerator] tickPrefab is null on {name}.",
                this
            );
            return false;
        }

        if (tickPrefab.GetComponent<TickView>() == null)
        {
            Debug.LogError(
                $"[TileGenerator] tickPrefab is missing TickView component on {name}.",
                this
            );
            return false;
        }

        if (!RandomizeStrengths())
            return false;

        if (!GenerateTicks())
            return false;

        ticksGenerated = true;
        return true;
    }

    private bool RandomizeStrengths()
    {
        const int maxAttempts = 500;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            attempts++;

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

            long totalTicks =
                (long)topStrength +
                rightStrength +
                bottomStrength +
                leftStrength;

            if (totalTicks >= (long)balanceConfig.minimumTotalTicks &&
                totalTicks <= (long)balanceConfig.maximumTotalTicks)
            {
                return true;
            }
        }

        Debug.LogError(
            $"[TileGenerator] Failed to generate valid strengths for {name} after {maxAttempts} attempts " +
            $"with side bounds [{balanceConfig.minimumSideStrength}, {balanceConfig.maximumSideStrength}] " +
            $"and total bounds [{balanceConfig.minimumTotalTicks}, {balanceConfig.maximumTotalTicks}].",
            this
        );

        return false;
    }

    private bool GenerateTicks()
    {
        if (!GenerateEdgeTicks(topStrength, Edge.Top)) return false;
        if (!GenerateEdgeTicks(rightStrength, Edge.Right)) return false;
        if (!GenerateEdgeTicks(bottomStrength, Edge.Bottom)) return false;
        if (!GenerateEdgeTicks(leftStrength, Edge.Left)) return false;

        return true;
    }

    private bool GenerateEdgeTicks(int tickCount, Edge edge)
    {
        if (tickCount == 0)
            return true;

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
                    return false;
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

                Destroy(tick);
                return false;
            }

            // Every tick on this tile receives the same color.
            tickView.Initialize(tileTickColor);
        }

        return true;
    }

    private enum Edge
    {
        Top,
        Right,
        Bottom,
        Left
    }
}
