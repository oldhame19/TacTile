using System.Collections.Generic;
using UnityEngine;

public enum PlayerId
{
    Player1,
    Player2
}

public enum TurnPhase
{
    BeginTurn,
    SelectTile,
    PlaceTile,
    EndTurn
}

public enum GameActionType
{
    BeginTurn,
    SelectTile,
    PlaceTile,
    EndTurn
}

public class GameState
{
    public PlayerId CurrentPlayer { get; set; } = PlayerId.Player1;

    public TurnPhase CurrentPhase { get; set; } = TurnPhase.BeginTurn;

    public TileView SelectedTile { get; set; }

    public List<BoardSpace> BoardSpaces { get; } =
        new List<BoardSpace>();

    public List<TileView> AllTiles { get; } =
        new List<TileView>();

    public int CompletedTurns { get; set; }

    public List<TileView> GetPlayerTiles(PlayerId player)
    {
        return AllTiles.FindAll(tile =>
            tile != null &&
            tile.Owner == player &&
            !tile.IsPlaced);
    }

    public List<BoardSpace> GetEmptySpaces()
    {
        return BoardSpaces.FindAll(space =>
            space != null &&
            !space.IsOccupied);
    }

    public bool IsGameOver
    {
        get
        {
            return GetEmptySpaces().Count == 0 ||
                   GetPlayerTiles(PlayerId.Player1).Count == 0 &&
                   GetPlayerTiles(PlayerId.Player2).Count == 0;
        }
    }
}