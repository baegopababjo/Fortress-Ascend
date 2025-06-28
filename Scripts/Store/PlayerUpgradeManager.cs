using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgradeManager : MonoBehaviour
{
    public static PlayerUpgradeManager Instance { get; private set; } // 싱글톤

    [Header("Player UI Elements")]
    public Text playerLevelText; // 🎯 UI에 표시될 플레이어 레벨
    public Text playerGoldText;  // 🎯 UI에 표시될 골드
    public Button upgradeButton; // 🎯 플레이어 업그레이드 버튼
    public Image playerImage;    // 🎯 UI에 표시될 캐릭터 이미지

    private PlayerStats playerStats; // 플레이어 정보 가져오기
    private int levelUpCost = 50; // 레벨업 비용 (고정)

    [Header("Character Sprites")] // 🔥 캐릭터별 이미지 매핑
    public Sprite warriorSprite; // 전사 이미지
    public Sprite archerSprite;  // 궁수 이미지

    private Dictionary<string, Sprite> characterImages; // 캐릭터 이미지 매핑

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
        playerStats = FindFirstObjectByType<PlayerStats>(); // ✅ 플레이어 객체 찾기

        if (playerStats == null)
        {
            Debug.LogWarning("⚠️ 플레이어가 아직 생성되지 않았습니다. 대기 중...");
            InvokeRepeating(nameof(CheckForPlayerStats), 0.5f, 0.5f); // 🔥 0.5초마다 플레이어 확인
            return;
        }

        upgradeButton.onClick.AddListener(UpgradePlayer);
        UpdateUI();
    }

    void CheckForPlayerStats()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();

        if (playerStats != null)
        {
            //Debug.Log("🎉 플레이어가 생성되었습니다! UI 업데이트 시작.");
            upgradeButton.onClick.AddListener(UpgradePlayer);
            UpdateUI();
            CancelInvoke(nameof(CheckForPlayerStats)); // 🔥 확인 반복 종료
        }
    }


    // 🎯 UI 정보 업데이트
    void UpdateUI()
    {
        if (playerStats == null)
        {
            Debug.LogWarning("⚠️ playerStats가 아직 초기화되지 않았습니다. 캐릭터 선택 후 다시 시도하세요.");
            return; // 🔥 UI 업데이트 중단
        }

        //플레이어의 레벨이 MAX Level 일 때
        if (playerStats.level == playerStats.MaxLevel)
        {
            playerLevelText.text = $"Level : {playerStats.level} (MAX)";
        }
        else
        {
            playerLevelText.text = $"Level : {playerStats.level}";
        }
        playerGoldText.text = $"Cost : {levelUpCost}";

        // 🔥 무기 타입을 기준으로 캐릭터 이미지 설정
        switch (playerStats.currentWeapon)
        {
            case PlayerStats.WeaponMode.Sword:
                playerImage.sprite = warriorSprite;
                //Debug.Log("✅ 전사(Warrior) 이미지 적용됨.");
                break;
            case PlayerStats.WeaponMode.Bow:
                playerImage.sprite = archerSprite;
                //Debug.Log("✅ 궁수(Archer) 이미지 적용됨.");
                break;
            default:
                Debug.LogWarning("⚠️ 알 수 없는 무기 타입입니다. 기본 이미지 적용 필요.");
                break;
        }
    }

    // 🔥 플레이어 업그레이드
    void UpgradePlayer()
    {
        if (playerStats.level >= playerStats.MaxLevel)
        {
            Debug.Log("최대 레벨");
        }
        else if (playerStats.gold >= levelUpCost)
        {
            playerStats.gold -= levelUpCost; // 골드 차감
            playerStats.SetLevelStats(playerStats.level + 1); // 레벨 증가

            UpdateUI(); // UI 업데이트
            Debug.Log($"🎉 플레이어 레벨업! 현재 레벨: {playerStats.level}");
        }
        else
        {
            Debug.Log("❌ 골드 부족!");
        }
    }

    // 🎯 외부에서 골드를 추가하는 함수
    public void AddGold(int amount)
    {
        playerStats.gold += amount;
        UpdateUI();
    }
}
