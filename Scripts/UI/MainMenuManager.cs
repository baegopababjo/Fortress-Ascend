using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button startButton;     // 🎮 게임 시작 버튼
    public Button settingButton;   // ⚙️ 설정 버튼
    public Button quitButton;      // 🚪 종료 버튼

    public GameObject gameSettingPanel; // 🎯 게임 설정 패널

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        settingButton.onClick.AddListener(OpenGameSettingPanel);
        quitButton.onClick.AddListener(QuitGame);
    }

    // 🎮 게임 시작
    void StartGame()
    {
        LoadingSceneManager.LoadScene("Map_EX");
    }

    // ⚙️ 설정 열기
    void OpenGameSettingPanel()
    {
        if (gameSettingPanel != null)
        {
            gameSettingPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("⚠️ GameSettingPanel이 설정되지 않았습니다.");
        }
    }

    // 🚪 게임 종료
    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
