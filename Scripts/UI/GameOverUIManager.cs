using UnityEngine;
using UnityEngine.UI;

public class GameOverUIManager : MonoBehaviour
{
    [Header("Game Over UI Elements")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Buttons")]
    public Button restartButton;
    public Button mainmenuButton;
    public Button quitButton;

    void Start()
    {
        // 버튼에 이벤트 연결
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (mainmenuButton != null)
            mainmenuButton.onClick.AddListener(OnMainMenuClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        // 게임 시작 시 패널은 꺼져 있어야 함
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            //Debug.Log("✅ GameOverPanel 활성화됨!");
        }
        else
        {
            Debug.LogError("❌ GameOverPanel이 연결되지 않았습니다.");
        }
    }

    void OnRestartClicked()
    {
        SettingsMenuManager.Instance.RestartGame();
    }

    void OnMainMenuClicked()
    {
        SettingsMenuManager.Instance.MainMenu();
    }

    void OnQuitClicked()
    {
        SettingsMenuManager.Instance.QuitGame();
    }
}
