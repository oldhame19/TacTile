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

        // BoardGenerator and TileSpawner run before this driver.
        BoardSpace[] spaces = FindObjectsOfType<BoardSpace>();
        TileView[] tiles = FindObjectsOfType<TileView>();

        foreach (BoardSpace space in spaces)
        {
            state.BoardSpaces.Add(space);
        }

        foreach (TileView tile in tiles)
        {
            // Reconnect each tile to the driver while preserving ownership.
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
                if (!space.TryPlaceTile(action.Tile))
                {
                    Debug.LogWarning(
                        "Placement failed: the space is occupied."
                    );

                    return;
                }

                Debug.Log(
                    state.CurrentPlayer + " placed " +
                    action.Tile.name + " at [" +
                    space.Row + "," + space.Column + "]"
                );

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