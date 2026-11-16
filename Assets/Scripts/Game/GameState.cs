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
                   (GetPlayerTiles(PlayerId.Player1).Count == 0 &&
                    GetPlayerTiles(PlayerId.Player2).Count == 0);
        }
    }

    public int GetPlayerScore(PlayerId player)
    {
        int score = 0;
        foreach (TileView tile in AllTiles)
        {
            if (tile != null && tile.IsPlaced && tile.Owner == player)
            {
                score++;
            }
        }
        return score;
    }

    public PlayerId? GetWinner(out int player1Score, out int player2Score)
    {
        player1Score = GetPlayerScore(PlayerId.Player1);
        player2Score = GetPlayerScore(PlayerId.Player2);

        if (player1Score > player2Score)
        {
            return PlayerId.Player1;
        }

        if (player2Score > player1Score)
        {
            return PlayerId.Player2;
        }

        return null;
    }
}