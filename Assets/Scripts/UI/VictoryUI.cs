using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
    [Header("UI Panels & Text")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private TMP_Text scoreSummaryText;

    [Header("Buttons")]
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button returnToMenuButton;

    [Header("Scene Navigation")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isSubscribed;

    private void OnEnable()
    {
        SubscribeToDriver();

        if (playAgainButton != null)
        {
            playAgainButton.onClick.RemoveListener(OnPlayAgainClicked);
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        }

        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.RemoveListener(OnReturnToMenuClicked);
            returnToMenuButton.onClick.AddListener(OnReturnToMenuClicked);
        }
    }

    private void Start()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        if (!isSubscribed)
        {
            SubscribeToDriver();
        }

        if (!isSubscribed)
        {
            Debug.LogError("[VictoryUI] GameDriver instance could not be found to subscribe to OnGameOver.");
        }
    }

    private void SubscribeToDriver()
    {
        if (isSubscribed)
        {
            return;
        }

        GameDriver driver = GameDriver.Instance;
        if (driver == null)
        {
            driver = FindFirstObjectByType<GameDriver>();
        }

        if (driver != null)
        {
            driver.OnGameOver += HandleGameOver;
            isSubscribed = true;
        }
    }

    private void OnDisable()
    {
        if (isSubscribed)
        {
            GameDriver driver = GameDriver.Instance;
            if (driver == null)
            {
                driver = FindFirstObjectByType<GameDriver>();
            }

            if (driver != null)
            {
                driver.OnGameOver -= HandleGameOver;
            }

            isSubscribed = false;
        }

        if (playAgainButton != null)
        {
            playAgainButton.onClick.RemoveListener(OnPlayAgainClicked);
        }

        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.RemoveListener(OnReturnToMenuClicked);
        }
    }

    private void HandleGameOver(PlayerId? winner, int p1Score, int p2Score)
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        if (winnerText != null)
        {
            if (winner == PlayerId.Player1)
            {
                winnerText.text = "PLAYER 1 WINS!";
            }
            else if (winner == PlayerId.Player2)
            {
                winnerText.text = "PLAYER 2 WINS!";
            }
            else
            {
                winnerText.text = "MATCH DRAW!";
            }
        }

        if (scoreSummaryText != null)
        {
            scoreSummaryText.text = $"Player 1: {p1Score}   —   Player 2: {p2Score}";
        }
    }

    private void OnPlayAgainClicked()
    {
        GameDriver driver = GameDriver.Instance;
        if (driver == null)
        {
            driver = FindFirstObjectByType<GameDriver>();
        }

        if (driver != null)
        {
            driver.RestartMatch();
        }
        else
        {
            Debug.LogError("[VictoryUI] Cannot restart match: GameDriver instance not found.");
        }
    }

    private void OnReturnToMenuClicked()
    {
        if (string.IsNullOrEmpty(mainMenuSceneName))
        {
            Debug.LogError("[VictoryUI] Cannot return to main menu: mainMenuSceneName is not set.", this);
            return;
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
