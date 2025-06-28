using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameStatsData gameStatsData;
    public NPC_Manager npcManager;

    public Text waveText;       
    public Text timerText;     

    private int currentWave = 0;
    private float waveTimer = 0f;
    private bool waveActive = false;
    private bool hasStartedGame = false;

    float TimeLimit;

    void Awake()
    {
        GameObject logoBGM = GameObject.Find("BackGroundMusic");
        if (logoBGM != null) { Destroy(logoBGM); }
    }

    public void StartGame()
    {
        if (gameStatsData == null)
        {
            gameStatsData = Resources.Load<GameStatsData>("GameStatsData");

            if (gameStatsData == null)
            {
                Debug.LogError("❌ GameStatsData를 Resources에서 찾을 수 없습니다. 연결 실패");
                return;
            }
            else
            {
                Debug.Log("✅ GameStatsData를 Resources에서 자동으로 불러왔습니다.");
            }
        }

        //Debug.Log("✅ gameStatsData 초기화 시작");
        gameStatsData.Initialize();
        CursorUtils.HideCursor();
        StartNextWave();
    }

    public IEnumerator StartGameDelayed()
    {
        yield return null;
        StartGame();
    }

    void Update()
    {
        if (!waveActive) return;

        waveTimer -= Time.deltaTime;
        timerText.text = $"Time Left: {Mathf.CeilToInt(waveTimer)} s";

        if (waveTimer <= 0f)
        {
            waveActive = false;
            StartNextWave();
        }
    }

    void StartNextWave()
    {
        currentWave += 1;
        var waveStats = gameStatsData.GetWaveStats(currentWave);

        if (waveStats == null)
        {
            Debug.LogError($"Wave stats for wave {currentWave} is null!");
            return;
        }
        if (npcManager == null)
        {
            npcManager = FindAnyObjectByType<NPC_Manager>(); // ✅ 자동 할당
            if (npcManager == null)
            {
                Debug.LogError("npcManager를 찾을 수 없습니다!");
                return;
            }
        }

        if (npcManager.gameStatsData == null)
        {
            npcManager.gameStatsData = gameStatsData;
            Debug.Log("✅ npcManager에 gameStatsData를 연결했습니다.");
        }

        if (waveText == null)
        {
            waveText = GameObject.Find("Wave")?.GetComponent<Text>();
        }
        if (timerText == null)
        {
            timerText = GameObject.Find("Time")?.GetComponent<Text>();
        }

        

        waveText.text = $"Wave {currentWave}";
        waveTimer = waveStats.timeLimit;
        waveActive = true;

        npcManager.WaveNPC(currentWave);
    }
    IEnumerator WaveProcess()
    {
        yield return new WaitForSeconds(TimeLimit);
    }

    public void GameOver()
    {
        //Debug.Log("💀 플레이어 사망! 게임 오버 처리 시작");
        Time.timeScale = 0f;

        // ✅ 메인 UI 끄기
        GameObject mainUIObj = GameObject.Find("MainUI"); // 이름에 맞게 찾아야 함!
        if (mainUIObj != null)
        {
            mainUIObj.SetActive(false);
            //Debug.Log("📴 메인 UI 비활성화됨 (하드코딩 방식)");
        }
        else
        {
            Debug.LogWarning("⚠ MainUI 오브젝트를 이름으로 찾을 수 없습니다.");
        }

        // ✅ GameOver UI 활성화
        GameOverUIManager uiManager = FindAnyObjectByType<GameOverUIManager>();
        CursorUtils.ShowCursor();
        if (uiManager != null)
        {
            uiManager.ShowGameOverPanel(); // 👉 직접 찾아서 호출
            //Debug.Log("✅ GameOverUIManager를 통해 GameOverPanel을 표시했습니다.");
        }
        else
        {
            Debug.LogError("❌ GameOverUIManager를 씬에서 찾을 수 없습니다.");
        }

        // ✅ 풀 리셋 추가
        if (EnemyPoolManager.Instance != null)
        {
            EnemyPoolManager.Instance.ResetAllPools();
        }
    }

}
