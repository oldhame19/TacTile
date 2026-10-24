
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class GameDriver : MonoBehaviour
{
    public static GameDriver Instance { get; private set; }

    private GameState state;

    public GameState State
    {
        get { return state; }
    }

    private enum TileSide
    {
        Top,
        Right,
        Bottom,
        Left
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        state = new GameState();

        BoardSpace[] spaces = FindObjectsOfType<BoardSpace>();
        TileView[] tiles = FindObjectsOfType<TileView>();

        foreach (BoardSpace space in spaces)
        {
            state.BoardSpaces.Add(space);
        }

        foreach (TileView tile in tiles)
        {
            tile.Initialize(tile.Owner, this);
            state.AllTiles.Add(tile);
        }

        Debug.Log("========== GAME INITIALIZED ==========");
        Debug.Log("Board spaces: " + state.BoardSpaces.Count);
        Debug.Log("Total tiles: " + state.AllTiles.Count);
        Debug.Log("Starting player: " + state.CurrentPlayer);

        if (state.BoardSpaces.Count == 0 || state.AllTiles.Count == 0)
        {
            Debug.LogError(
                "Game initialization failed. Check your board and tile prefabs."
            );

            enabled = false;
            return;
        }

        LogCurrentPhase();
        AdvanceAutomaticPhases();
    }

    public void TrySelectTile(TileView tile)
    {
        if (state == null || tile == null)
        {
            return;
        }

        List<LegalAction> actions =
            LegalMoveGenerator.GetLegalActions(
                state,
                state.CurrentPlayer
            );

        foreach (LegalAction action in actions)
        {
            if (action.Type == GameActionType.SelectTile &&
                action.Tile == tile)
            {
                state.SelectedTile = tile;

                Debug.Log(
                    state.CurrentPlayer + " selected " + tile.name
                );

                if (TurnStateMachine.Advance(state))
                {
                    LogCurrentPhase();
                }

                return;
            }
        }

        Debug.Log(
            "Illegal selection: " + tile.name +
            ". Current player: " + state.CurrentPlayer +
            ". Current phase: " + state.CurrentPhase
        );
    }

    public void TryPlaceTile(BoardSpace space)
    {
        if (state == null || space == null)
        {
            return;
        }

        List<LegalAction> actions =
            LegalMoveGenerator.GetLegalActions(
                state,
                state.CurrentPlayer
            );

        foreach (LegalAction action in actions)
        {
            if (action.Type == GameActionType.PlaceTile &&
                action.Space == space)
            {
                TileView placedTile = action.Tile;

                if (!space.TryPlaceTile(placedTile))
                {
                    Debug.LogWarning(
                        "Placement failed: the space is occupied."
                    );

                    return;
                }

                Debug.Log(
                    state.CurrentPlayer + " placed " +
                    placedTile.name + " at [" +
                    space.Row + "," + space.Column + "]"
                );

                // Resolve captures immediately after placement.
                ResolveCaptures(placedTile, space);

                if (TurnStateMachine.Advance(state))
                {
                    LogCurrentPhase();
                    AdvanceAutomaticPhases();
                }

                return;
            }
        }

        Debug.Log(
            "Illegal placement at [" + space.Row + "," +
            space.Column + "]. Current phase: " +
            state.CurrentPhase
        );
    }

    private void ResolveCaptures(
        TileView placedTile,
        BoardSpace placedSpace)
    {
        if (placedTile == null || placedSpace == null)
        {
            return;
        }

        // Rows increase toward positive Z in BoardGenerator.
        // Columns increase toward positive X.
        CheckAdjacentCapture(
            placedTile,
            placedSpace.Row + 1,
            placedSpace.Column,
            TileSide.Top,
            TileSide.Bottom
        );

        CheckAdjacentCapture(
            placedTile,
            placedSpace.Row,
            placedSpace.Column + 1,
            TileSide.Right,
            TileSide.Left
        );

        CheckAdjacentCapture(
            placedTile,
            placedSpace.Row - 1,
            placedSpace.Column,
            TileSide.Bottom,
            TileSide.Top
        );

        CheckAdjacentCapture(
            placedTile,
            placedSpace.Row,
            placedSpace.Column - 1,
            TileSide.Left,
            TileSide.Right
        );
    }

    private void CheckAdjacentCapture(
        TileView attackingTile,
        int targetRow,
        int targetColumn,
        TileSide attackingSide,
        TileSide defendingSide)
    {
        BoardSpace adjacentSpace = state.BoardSpaces.Find(
            candidate =>
                candidate != null &&
                candidate.Row == targetRow &&
                candidate.Column == targetColumn
        );

        // There is no space here, or the space is empty.
        if (adjacentSpace == null ||
            !adjacentSpace.IsOccupied)
        {
            return;
        }

        TileView defendingTile = adjacentSpace.OccupyingTile;

        // Only capture opposing tiles.
        if (defendingTile == null ||
            defendingTile.Owner == attackingTile.Owner)
        {
            return;
        }

        int attackingStrength = GetEffectiveStrength(
            attackingTile,
            attackingSide
        );

        int defendingStrength = GetEffectiveStrength(
            defendingTile,
            defendingSide
        );

        Debug.Log(
            "Comparing " + attackingTile.name + " (" +
            attackingSide + ": " + attackingStrength +
            ") against " + defendingTile.name + " (" +
            defendingSide + ": " + defendingStrength + ")"
        );

        // Equal strength does not capture.
        if (attackingStrength > defendingStrength)
        {
            PlayerId previousOwner = defendingTile.Owner;

            defendingTile.SetOwner(attackingTile.Owner);

            Debug.Log(
                attackingTile.name + " captured " +
                defendingTile.name + " from " +
                previousOwner + ". New owner: " +
                defendingTile.Owner
            );
        }
    }

    private int GetEffectiveStrength(
        TileView tile,
        TileSide side)
    {
        TileGenerator generator =
            tile.GetComponent<TileGenerator>();

        if (generator == null)
        {
            Debug.LogError(
                "Tile is missing TileGenerator: " + tile.name,
                tile
            );

            return 0;
        }

        int baseStrength;

        switch (side)
        {
            case TileSide.Top:
                baseStrength = generator.topStrength;
                break;

            case TileSide.Right:
                baseStrength = generator.rightStrength;
                break;

            case TileSide.Bottom:
                baseStrength = generator.bottomStrength;
                break;

            case TileSide.Left:
                baseStrength = generator.leftStrength;
                break;

            default:
                return 0;
        }

        // A tile receives its boost while occupying a boost space.
        BoardSpace currentSpace = tile.CurrentSpace;

        if (currentSpace == null || !currentSpace.IsBoostSpace)
        {
            return baseStrength;
        }

        BoostType boost = currentSpace.Boost;

        // The neutral center boost applies to every tick color.
        if (boost == BoostType.Neutral)
        {
            return baseStrength + 1;
        }

        // Colored boosts apply only to matching tile tick colors.
        bool matchingColor =
            (boost == BoostType.Red &&
             generator.TileTickColor == TickColor.Red) ||

            (boost == BoostType.LightBlue &&
             generator.TileTickColor == TickColor.LightBlue) ||

            (boost == BoostType.Yellow &&
             generator.TileTickColor == TickColor.Yellow);

        return matchingColor
            ? baseStrength + 1
            : baseStrength;
    }

    private void AdvanceAutomaticPhases()
    {
        while (state != null &&
               (state.CurrentPhase == TurnPhase.BeginTurn ||
                state.CurrentPhase == TurnPhase.EndTurn))
        {
            if (!TurnStateMachine.Advance(state))
            {
                Debug.LogError("Invalid state-machine transition.");
                return;
            }

            LogCurrentPhase();
        }
    }

    private void LogCurrentPhase()
    {
        if (state == null)
        {
            return;
        }

        Debug.Log(
            "========== " + state.CurrentPhase +
            " | Active player: " + state.CurrentPlayer +
            " | Completed turns: " + state.CompletedTurns +
            " =========="
        );

        List<LegalAction> actions =
            LegalMoveGenerator.GetLegalActions(
                state,
                state.CurrentPlayer
            );

        Debug.Log("Legal actions (" + actions.Count + "):");

        if (actions.Count == 0)
        {
            Debug.Log("(No legal actions available.)");
        }

        foreach (LegalAction action in actions)
        {
            Debug.Log("  " + action);
        }
    }
}
