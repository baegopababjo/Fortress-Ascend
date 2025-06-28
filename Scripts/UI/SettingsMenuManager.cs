using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenuManager : MonoBehaviour
{
    public static SettingsMenuManager Instance { get; private set; } // 싱글톤

    public GameObject settingsMenu; // 🎯 ESC 환경설정 메뉴 패널
    public Button continueButton;
    public Button restartButton;
    public Button quitButton;
    public Button settingButton;            // ⚙️ 설정 버튼
    public GameObject gameSettingPanel;     // 🎮 해상도, 볼륨 게임 설정 패널

    private bool isMenuOpen = false; // ✅ 메뉴가 열려 있는지 여부

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 🔥 버튼 이벤트 연결
        continueButton.onClick.AddListener(ContinueGame);
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
        settingButton.onClick.AddListener(OpenGameSettingPanel); // ✅ 설정 버튼 클릭 시

        // 🎯 시작할 때 설정 메뉴 숨기기
        settingsMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameSettingPanel.activeSelf)
            {
                CursorUtils.HideCursor();
                gameSettingPanel.SetActive(false);
                settingsMenu.SetActive(false);
                isMenuOpen = false;
                Time.timeScale = 1f;
            }
            else
            {
                ToggleSettingsMenu();
            }
        }
    }

    // 🔥 설정 메뉴 토글
    void ToggleSettingsMenu()
    {
        isMenuOpen = !isMenuOpen;
        settingsMenu.SetActive(isMenuOpen);

        Time.timeScale = isMenuOpen ? 0f : 1f;

        if (isMenuOpen)
            CursorUtils.ShowCursor();
        else
            CursorUtils.HideCursor();

        Debug.Log(isMenuOpen ? "⏸ 게임 정지됨 (설정 메뉴 열림)" : "▶ 게임 재개됨 (설정 메뉴 닫힘)");
    }

    // 🎮 계속하기 (설정 창 닫기)
    void ContinueGame()
    {
        isMenuOpen = false;
        settingsMenu.SetActive(false);
        Time.timeScale = 1f; // ⏯ 게임 재개
        CursorUtils.HideCursor();
    }

    // 🔄 다시하기 (현재 씬 재시작)
    public void RestartGame()
    {
        Time.timeScale = 1f; // 혹시 모르니까 시간도 초기화

        // ✅ 상태 리셋 오브젝트 제거 (선택적)
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Destroy(player); // 죽은 캐릭터 제거
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 메인 메뉴로 돌아가기
    public void MainMenu()
    {
        SceneManager.LoadScene("LogoScene");
    }

    // 🚪 그만하기 (게임 종료)
    public void QuitGame()
    {
    #if UNITY_EDITOR
            // Unity 에디터에서 Play 모드 종료
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            // 빌드된 게임에서 종료
            Application.Quit();
    #endif
    }

    // 🔥 메뉴가 열려 있는지 확인하는 함수
    public bool IsMenuOpen()
    {
        return isMenuOpen;
    }

    void OpenGameSettingPanel()
    {
        gameSettingPanel.SetActive(true);
    }

}
