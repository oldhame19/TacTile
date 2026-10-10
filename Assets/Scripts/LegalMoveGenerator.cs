using System.Collections.Generic;

public class LegalAction
{
    public GameActionType Type { get; private set; }

    public TileView Tile { get; private set; }

    public BoardSpace Space { get; private set; }

    public LegalAction(
        GameActionType type,
        TileView tile = null,
        BoardSpace space = null)
    {
        Type = type;
        Tile = tile;
        Space = space;
    }

    public override string ToString()
    {
        switch (Type)
        {
            case GameActionType.BeginTurn:
                return "BeginTurn";

            case GameActionType.SelectTile:
                return "SelectTile: " + Tile.name;

            case GameActionType.PlaceTile:
                return "PlaceTile: " + Tile.name +
                       " -> Space[" + Space.Row +
                       "," + Space.Column + "]";

            case GameActionType.EndTurn:
                return "EndTurn";

            default:
                return "Unknown action";
        }
    }
}

public static class LegalMoveGenerator
{
    public static List<LegalAction> GetLegalActions(
        GameState state,
        PlayerId player)
    {
        List<LegalAction> actions = new List<LegalAction>();

        if (state == null || player != state.CurrentPlayer)
        {
            return actions;
        }

        switch (state.CurrentPhase)
        {
            case TurnPhase.BeginTurn:
                actions.Add(
                    new LegalAction(GameActionType.BeginTurn));
                break;

            case TurnPhase.SelectTile:
                foreach (TileView tile in state.GetPlayerTiles(player))
                {
                    actions.Add(new LegalAction(
                        GameActionType.SelectTile,
                        tile));
                }
                break;

            case TurnPhase.PlaceTile:
                TileView selected = state.SelectedTile;

                if (selected == null ||
                    selected.Owner != player ||
                    selected.IsPlaced)
                {
                    break;
                }

                foreach (BoardSpace space in state.GetEmptySpaces())
                {
                    actions.Add(new LegalAction(
                        GameActionType.PlaceTile,
                        selected,
                        space));
                }
                break;

            case TurnPhase.EndTurn:
                actions.Add(
                    new LegalAction(GameActionType.EndTurn));
                break;
        }

        return actions;
    }
}