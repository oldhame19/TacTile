using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene Navigation")]
    [SerializeField] private string gameplaySceneName = "MainScene";

    [Header("Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button rulesButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveListener(OnPlayClicked);
            playButton.onClick.AddListener(OnPlayClicked);
        }
        else
        {
            Debug.LogWarning("[MainMenuUI] Play button reference is not assigned.", this);
        }

        if (rulesButton != null)
        {
            rulesButton.onClick.RemoveListener(OnRulesClicked);
            rulesButton.onClick.AddListener(OnRulesClicked);
        }
        else
        {
            Debug.LogWarning("[MainMenuUI] Rules button reference is not assigned.", this);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveListener(OnSettingsClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
        }
        else
        {
            Debug.LogWarning("[MainMenuUI] Settings button reference is not assigned.", this);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(OnQuitClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
        }
        else
        {
            Debug.LogWarning("[MainMenuUI] Quit button reference is not assigned.", this);
        }
    }

    private void OnDisable()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveListener(OnPlayClicked);
        }

        if (rulesButton != null)
        {
            rulesButton.onClick.RemoveListener(OnRulesClicked);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveListener(OnSettingsClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(OnQuitClicked);
        }
    }

    private void OnPlayClicked()
    {
        if (string.IsNullOrEmpty(gameplaySceneName))
        {
            Debug.LogError("[MainMenuUI] Cannot load gameplay scene: gameplaySceneName is not set.", this);
            return;
        }

        Debug.Log($"[MainMenuUI] Loading gameplay scene: {gameplaySceneName}");
        SceneManager.LoadScene(gameplaySceneName);
    }

    private void OnRulesClicked()
    {
        Debug.Log("[MainMenuUI] Rules screen will be implemented in Milestone 4.2.");
    }

    private void OnSettingsClicked()
    {
        Debug.Log("[MainMenuUI] Settings screen will be implemented in Milestone 4.3.");
    }

    private void OnQuitClicked()
    {
        Debug.Log("[MainMenuUI] Quitting game application...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
