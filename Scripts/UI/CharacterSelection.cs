using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public static CharacterSelection Instance; // ✅ 싱글톤 인스턴스
    [SerializeField] private HpUIManager hpUIManager;

    public GameStatsData gameStatsData;
    public GameObject warriorPrefab;    // 전사 프리팹
    public GameObject archerPrefab;     // 궁수 프리팹
    public Transform spawnPoint;        // 캐릭터 생성 위치
    public GameObject selectionUI;      // 캐릭터 선택 UI
    public Player_Camera playerCamera;  // 메인 카메라 스크립트

    private GameObject activePlayer;    // 현재 선택된 플레이어

    public GameObject mainUI;           // ✅ 메인 UI (골드, 미니맵, 체력바 등)

    public GameObject magicSelectionUI; // ✅ 마법 선택 UI (Panel 2)
    private PlayerStats.MagicMode selectedMagicType;   // ✅ 선택된 마법 종류 ("강화마법" 또는 "먼지마법")

    public bool isMagicSelected { get; private set; } = false; // ✅ 외부 접근 허용용

    void Awake()
    {
        // ✅ 싱글톤 초기화
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        selectionUI.SetActive(true);    // 선택 UI 활성화
        mainUI.SetActive(false);        // ✅ 메인 UI 숨기기
    }

    public void SelectWarrior()
    {
        SpawnCharacter(warriorPrefab);
    }

    public void SelectArcher()
    {
        SpawnCharacter(archerPrefab);
    }

    void SpawnCharacter(GameObject characterPrefab)
    {
        if (activePlayer != null) Destroy(activePlayer); // 기존 캐릭터 삭제

        // 캐릭터 생성 & 활성화
        activePlayer = Instantiate(characterPrefab, spawnPoint.position, spawnPoint.rotation);
        activePlayer.SetActive(true); // ✅ 강제 활성화

        // UI 연결
        if (hpUIManager != null)
        {
            hpUIManager.SetTarget(activePlayer);
        }
        else
        {
            Debug.LogError("❌ HpUIManager가 연결되지 않았습니다!");
        }



        // 카메라가 선택한 캐릭터를 따라가도록 설정
        playerCamera.Player = activePlayer.transform;

        // UI 전환
        selectionUI.SetActive(false);       // ✅ 선택 UI 숨기기
        magicSelectionUI.SetActive(true);   // ✅ 마법 선택 UI 보여주기 (Panel 2)
        //mainUI.SetActive(true);           // ✅ 메인 UI 활성화

        // ✅ 코루틴이 안 되는 CharacterSelection 대신 GameManager에서 실행
        //StartGameAfterUIReady();
    }


    public void SelectBuffMagic()
    {
        selectedMagicType = PlayerStats.MagicMode.EmpoweredAttack; // 강화 마법
        FinalizeSelection();
    }

    public void SelectDustMagic()
    {
        selectedMagicType = PlayerStats.MagicMode.MeleeMagic; // 먼지 마법
        FinalizeSelection();
    }


    void FinalizeSelection()
    {
        magicSelectionUI.SetActive(false);  // 마법 선택 UI 숨기기
        mainUI.SetActive(true);             // 메인 UI 켜기

        Debug.Log($"🪄 선택된 마법: {selectedMagicType}");

        // 🧙 선택한 마법을 PlayerStats에 전달
        var playerStats = activePlayer.GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            playerStats.currentMagic = selectedMagicType;
            Debug.Log("✅ 마법 타입이 PlayerStats에 적용됨");
        }
        else
        {
            Debug.LogError("❌ activePlayer에 PlayerStats가 없습니다.");
        }

        isMagicSelected = true; // ✅ 마법 선택 완료 플래그 설정
        StartGameAfterUIReady();
    }


    void StartGameAfterUIReady()
    {
        var gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null)
        {
            // ✅ StartGame 실행 전에 gameStatsData를 강제로 연결
            if (gameManager.gameStatsData == null && gameStatsData != null)
            {
                gameManager.gameStatsData = gameStatsData;
                Debug.Log("✅ gameStatsData가 GameManager에 연결되었습니다!");
            }

            gameManager.StartCoroutine(gameManager.StartGameDelayed());
        }
    }

}
