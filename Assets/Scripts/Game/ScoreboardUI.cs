
using TMPro;
using UnityEngine;

public class ScoreboardUI : MonoBehaviour
{
    [Header("Player 1 UI")]
    [SerializeField] private TMP_Text player1NameText;
    [SerializeField] private TMP_Text player1ScoreText;

    [Header("Player 2 UI")]
    [SerializeField] private TMP_Text player2NameText;
    [SerializeField] private TMP_Text player2ScoreText;

    [Header("Active Player Highlight")]
    [SerializeField]
    private Color activePlayerColor =
        new Color(1f, 0.8f, 0.2f);

    [SerializeField]
    private Color inactivePlayerColor =
        Color.white;

    private int lastPlayer1Score = -1;
    private int lastPlayer2Score = -1;

    private PlayerId lastActivePlayer;
    private bool hasCachedActivePlayer;

    

    private void Update()
    {
        GameDriver driver = GameDriver.Instance;

        if (driver == null || driver.State == null)
        {
            return;
        }

        GameState state = driver.State;

        int player1Score = 0;
        int player2Score = 0;

        // Count only tiles currently placed on the board.
        // Ownership determines which player's score increases.
        foreach (TileView tile in state.AllTiles)
        {
            if (tile == null || !tile.IsPlaced)
            {
                continue;
            }

            if (tile.Owner == PlayerId.Player1)
            {
                player1Score++;
            }
            else if (tile.Owner == PlayerId.Player2)
            {
                player2Score++;
            }
        }

        // Update the displayed scores only when they change.
        if (player1Score != lastPlayer1Score)
        {
            player1ScoreText.text = player1Score.ToString();
            lastPlayer1Score = player1Score;
        }

        if (player2Score != lastPlayer2Score)
        {
            player2ScoreText.text = player2Score.ToString();
            lastPlayer2Score = player2Score;
        }

        // Update the highlight when the active player changes.
        if (!hasCachedActivePlayer ||
            state.CurrentPlayer != lastActivePlayer)
        {
            UpdateActivePlayerHighlight(state.CurrentPlayer);

            lastActivePlayer = state.CurrentPlayer;
            hasCachedActivePlayer = true;
        }
    }

    private void UpdateActivePlayerHighlight(PlayerId activePlayer)
    {
        bool player1IsActive =
            activePlayer == PlayerId.Player1;

        player1NameText.color = player1IsActive
            ? activePlayerColor
            : inactivePlayerColor;

        player2NameText.color = player1IsActive
            ? inactivePlayerColor
            : activePlayerColor;

    }
}
