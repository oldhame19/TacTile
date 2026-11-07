
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
    public int tilesPerPlayer = 8;

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
}
