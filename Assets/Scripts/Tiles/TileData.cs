using UnityEngine;

[CreateAssetMenu(fileName = "NewTile", menuName = "TacTile/Tile")]
public class TileData : ScriptableObject
{
    public int id;
    public string displayName;

    [Header("Edge Values")]
    public int top;
    public int right;
    public int bottom;
    public int left;
}