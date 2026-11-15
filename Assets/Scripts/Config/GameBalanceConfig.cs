
using UnityEngine;

[CreateAssetMenu(
    fileName = "GameBalanceConfig",
    menuName = "TacTile/Game Balance Config"
)]
public class GameBalanceConfig : ScriptableObject
{
    [Header("Board Settings")]
    [Min(3)]
    public int boardSize = 5;

    [Min(0)]
    public int boostsPerColor = 2;

    [Header("Player Settings")]
    [Min(1)]
    public int tilesPerPlayer = 13;

    [Header("Tile Strength Settings")]
    [Min(0)]
    public int minimumSideStrength = 0;

    [Min(0)]
    public int maximumSideStrength = 4;

    [Min(0)]
    public int minimumTotalTicks = 7;

    [Min(0)]
    public int maximumTotalTicks = 9;

    [Header("Boost Strength")]
    [Min(0)]
    public int neutralBoostBonus = 1;

    [Min(0)]
    public int matchingColorBoostBonus = 1;

    public bool ValidateConfig(out string errorMessage)
    {
        if (minimumSideStrength < 0)
        {
            errorMessage = $"minimumSideStrength cannot be negative ({minimumSideStrength}).";
            return false;
        }

        if (maximumSideStrength < 0)
        {
            errorMessage = $"maximumSideStrength cannot be negative ({maximumSideStrength}).";
            return false;
        }

        if (minimumTotalTicks < 0)
        {
            errorMessage = $"minimumTotalTicks cannot be negative ({minimumTotalTicks}).";
            return false;
        }

        if (maximumTotalTicks < 0)
        {
            errorMessage = $"maximumTotalTicks cannot be negative ({maximumTotalTicks}).";
            return false;
        }

        if (boostsPerColor < 0)
        {
            errorMessage = $"boostsPerColor cannot be negative ({boostsPerColor}).";
            return false;
        }

        if (neutralBoostBonus < 0)
        {
            errorMessage = $"neutralBoostBonus cannot be negative ({neutralBoostBonus}).";
            return false;
        }

        if (matchingColorBoostBonus < 0)
        {
            errorMessage = $"matchingColorBoostBonus cannot be negative ({matchingColorBoostBonus}).";
            return false;
        }

        if (tilesPerPlayer < 1)
        {
            errorMessage = $"tilesPerPlayer must be at least 1 ({tilesPerPlayer}).";
            return false;
        }

        if (boardSize < 3)
        {
            errorMessage = $"boardSize must be at least 3 ({boardSize}).";
            return false;
        }

        if (boardSize % 2 == 0)
        {
            errorMessage = $"boardSize must be an odd number to provide a center boost space ({boardSize}).";
            return false;
        }

        long availableSpacesForBoosts = ((long)boardSize * boardSize) - 1L;
        long requiredBoostSpaces = (long)boostsPerColor * 3L;
        if (requiredBoostSpaces > availableSpacesForBoosts)
        {
            errorMessage = $"Not enough board spaces ({availableSpacesForBoosts}) for {boostsPerColor} boosts per color ({requiredBoostSpaces} required).";
            return false;
        }

        if (minimumSideStrength > maximumSideStrength)
        {
            errorMessage = $"minimumSideStrength ({minimumSideStrength}) cannot exceed maximumSideStrength ({maximumSideStrength}).";
            return false;
        }

        if (minimumTotalTicks > maximumTotalTicks)
        {
            errorMessage = $"minimumTotalTicks ({minimumTotalTicks}) cannot exceed maximumTotalTicks ({maximumTotalTicks}).";
            return false;
        }

        long minPossibleTotal = (long)minimumSideStrength * 4L;
        if (minPossibleTotal > (long)maximumTotalTicks)
        {
            errorMessage = $"Minimum possible total ticks ({minPossibleTotal} = 4 * {minimumSideStrength}) exceeds maximumTotalTicks ({maximumTotalTicks}).";
            return false;
        }

        long maxPossibleTotal = (long)maximumSideStrength * 4L;
        if (maxPossibleTotal < (long)minimumTotalTicks)
        {
            errorMessage = $"Maximum possible total ticks ({maxPossibleTotal} = 4 * {maximumSideStrength}) is less than minimumTotalTicks ({minimumTotalTicks}).";
            return false;
        }

        errorMessage = null;
        return true;
    }
}
