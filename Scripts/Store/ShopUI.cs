using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    public static bool IsShopOpen = false; // 🔥 상점이 열려 있는지 여부
    private Animator playerAnimator;

    [Header("Game Data")]
    public GameStatsData gameStatsData; // 📌 빌딩 데이터를 가져올 ScriptableObject
    public PlayerStats playerStats; // 🔥 PlayerStats 참조 추가

    [Header("UI Elements")]
    public GameObject shopPanel;
    public Button closeButton;

    [Header("Tabs")]
    public Button castleTab;
    public Button buildingTab;
    public Button npcTab;
    public Button playerTab;
    public GameObject castlePanel;
    public GameObject buildingPanel;
    public GameObject npcPanel;
    public GameObject playerPanel;

    [Header("Castle Building UI")]
    public Text[] castlePriceTexts;              // 가격 텍스트 (HP 및 가격 표시)
    public Text[] castleLevelTexts;              // 현재 레벨 텍스트
    public Text[] castleUpgradeCostTexts;        // 업그레이드 가격 텍스트
    public Button[] castleUpgradeButtons;        // 업그레이드 버튼


    [Header("Building Upgrade UI")]
    public Text[] buildingNameTexts;
    public Text[] buildingLevelTexts;
    public Text[] buildingCostTexts;
    public Text[] buildingUpgradeCostTexts;
    public Button[] buyBuildingButtons;
    public Button[] upgradeBuildingButtons;

    private Dictionary<string, int> buildingLevels = new Dictionary<string, int>(); // 현재 빌딩 레벨 저장
    private Dictionary<string, int> npcLevels = new Dictionary<string, int>(); // 현재 NPC 레벨 저장
    private Dictionary<string, int> castleBuildingLevels = new Dictionary<string, int>(); // 💾 현재 Castle 레벨 저장용

    [Header("Building Placement")]
    public BuildingPlacementManager placementManager; // 📌 새로 추가된 건물 배치 매니저

    [Header("Slot 선택용 MAP UI")]
    public GameObject MAP_BuildingC; // 빌딩 C 전용 MAP UI
    public GameObject MAP_BuildingD; // 빌딩 D 전용 MAP UI
    public Button[] wallSlotButtons; // C 빌딩용 슬롯 (Temp_Wall1~3)
    public Button[] towerSlotButtons; // D 빌딩용 슬롯 (Temp_Tower1~3)

    private string pendingBuildingName; // 어떤 빌딩이 선택됐는지 저장
    private int pendingBuildingLevel;
    private int pendingBuildingHealth;

    [Header("NPC Upgrade UI")]
    public Text[] npcCostTexts;         
    public Text[] npcLevelTexts;       
    public Text[] npcStatTexts;
    public Text[] npcUpgradeCostTexts;
    public Button[] buyNpcButtons;      // 구매 버튼
    public Button[] upgradeNpcButtons;  // 업그레이드 버튼

    [Header("NPC Placement")]
    public NpcPlacementManager npcPlacementManager;




    void Start()
    {
        if (gameStatsData == null)
        {
            Debug.LogError("❌ GameStatsData가 할당되지 않았습니다! Inspector에서 연결하세요.");
            return;
        }

        // 🔹 ScriptableObject 강제 초기화
        gameStatsData.Initialize();

        // 🎯 "Player" 레이어 오브젝트 찾기
        FindPlayerStats();

        shopPanel.SetActive(false);
        closeButton.onClick.AddListener(() => shopPanel.SetActive(false));

        castleTab.onClick.AddListener(() => SwitchTab("Castle"));
        buildingTab.onClick.AddListener(() => SwitchTab("Building"));
        npcTab.onClick.AddListener(() => SwitchTab("NPC"));
        playerTab.onClick.AddListener(() => SwitchTab("Player"));

        // ✅ 여기부터 NPC 버튼 연결
        buyNpcButtons[0].onClick.AddListener(() => BuyNpc("Warrior"));
        buyNpcButtons[1].onClick.AddListener(() => BuyNpc("Archer"));

        // ✅ 빌딩 C/D 맵 비활성화
        MAP_BuildingC.SetActive(false);
        MAP_BuildingD.SetActive(false);

        // 빌딩 C 전용 Wall 슬롯 버튼 연결
        wallSlotButtons[0].onClick.AddListener(() => OnSlotSelected("LongWall_F1", "Slot_1"));
        wallSlotButtons[1].onClick.AddListener(() => OnSlotSelected("LongWall_F1", "Slot_2"));
        wallSlotButtons[2].onClick.AddListener(() => OnSlotSelected("LongWall_F2", "Slot_1"));
        wallSlotButtons[3].onClick.AddListener(() => OnSlotSelected("LongWall_F2", "Slot_2"));
        wallSlotButtons[4].onClick.AddListener(() => OnSlotSelected("LongWall_L1", "Slot_1"));
        wallSlotButtons[5].onClick.AddListener(() => OnSlotSelected("LongWall_L1", "Slot_2"));
        wallSlotButtons[6].onClick.AddListener(() => OnSlotSelected("ShortWall_B1", "Slot_1"));
        wallSlotButtons[7].onClick.AddListener(() => OnSlotSelected("LongWall_B1", "Slot_1"));
        wallSlotButtons[8].onClick.AddListener(() => OnSlotSelected("LongWall_B1", "Slot_2"));
        wallSlotButtons[9].onClick.AddListener(() => OnSlotSelected("LongWall_R1", "Slot_1"));
        wallSlotButtons[10].onClick.AddListener(() => OnSlotSelected("LongWall_R1", "Slot_2"));


        // 빌딩 D 전용 Tower 슬롯 버튼 연결
        towerSlotButtons[0].onClick.AddListener(() => OnSlotSelected("SmallTower_R", "Slot_1"));
        towerSlotButtons[1].onClick.AddListener(() => OnSlotSelected("SmallTower_L", "Slot_1"));
        towerSlotButtons[2].onClick.AddListener(() => OnSlotSelected("BigTower_R", "Slot_1"));
        towerSlotButtons[3].onClick.AddListener(() => OnSlotSelected("BigTower_L", "Slot_1"));
        towerSlotButtons[4].onClick.AddListener(() => OnSlotSelected("Wooden_Tower(Building A)", "Out"));


        InitializeCastleBuildings();//Castle UI 초기화
        InitializeBuildings();      // 빌딩 UI 초기화
        InitializeNpcs();           // NPC UI 초기화!

        SwitchTab("Castle"); // 기본적으로 메인 건물 탭 활성화
    }

    void Update()
    {
        // ⏳ Start에서 못 찾았을 경우, Update에서 한 번 더 찾기
        if (playerStats == null)
        {
            FindPlayerStats();
        }


        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (IsShopOpen && playerAnimator != null)
            {
                playerAnimator.SetBool("isWalking", false);
            }
            ToggleShop();
        }

        
    }

    void FindPlayerStats()
    {
        int playerLayer = LayerMask.NameToLayer("Player"); // 🎯 "Player" 레이어 인덱스
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None); // 🔥 최적화된 검색

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == playerLayer)
            {
                playerStats = obj.GetComponent<PlayerStats>();
                playerAnimator = obj.GetComponent<Animator>();
                if (playerStats != null)
                {
                   // Debug.Log($"✅ PlayerStats 할당 완료! {obj.name}에서 찾음.");
                    return;
                }
            }
        }

        //Debug.LogWarning("⚠️ 아직 PlayerStats를 찾지 못했습니다. Update에서 다시 확인합니다.");
    }

    public void ToggleShop()
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
        IsShopOpen = shopPanel.activeSelf;
        if (IsShopOpen)
            CursorUtils.ShowCursor();  // 상점 열릴 때 커서 보이기
        else
            CursorUtils.HideCursor();  // 상점 닫힐 때 커서 숨기기
    }

    void SwitchTab(string tab)
    {
        castlePanel.SetActive(tab == "Castle");
        buildingPanel.SetActive(tab == "Building");
        npcPanel.SetActive(tab == "NPC");
        playerPanel.SetActive(tab == "Player");
    }

    void InitializeCastleBuildings()
    {
        List<string> buildingNames = new() { "MainBuilding", "Building 1", "Building 2" };

        for (int i = 0; i < buildingNames.Count; i++)
        {
            string buildingName = buildingNames[i];

            if (!castleBuildingLevels.ContainsKey(buildingName))
                castleBuildingLevels[buildingName] = 1;

            int level = castleBuildingLevels[buildingName];
            var (health, cost) = gameStatsData.GetDefaultBuildingStats(buildingName, level);

            castleLevelTexts[i].text = $"Level: {level}";
            castlePriceTexts[i].text = $"HP: {health} / Cost: {cost}";

            string upgradeText = "Max Level";
            if (level < 5)
            {
                var (_, upgradeCost) = gameStatsData.GetDefaultBuildingStats(buildingName, level + 1);
                upgradeText = $"Upgrade Cost: {upgradeCost}";
            }
            castleUpgradeCostTexts[i].text = upgradeText;

            int capturedIndex = i;

            castleUpgradeButtons[i].onClick.RemoveAllListeners();
            castleUpgradeButtons[i].onClick.AddListener(() => UpgradeCastleBuilding(buildingName, capturedIndex));
        }
    }

    void UpgradeCastleBuilding(string buildingName, int uiIndex)
    {
        int level = castleBuildingLevels[buildingName];

        if (level >= 5)
        {
            Debug.Log("⚠️ 이미 최대 레벨입니다!");
            return;
        }

        var (newHealth, upgradeCost) = gameStatsData.GetDefaultBuildingStats(buildingName, level + 1);

        if (playerStats.gold < upgradeCost)
        {
            Debug.Log("❌ 골드 부족!");
            return;
        }

        // 골드 차감 + 레벨 증가
        playerStats.gold -= upgradeCost;
        castleBuildingLevels[buildingName]++;
        int newLevel = castleBuildingLevels[buildingName];

        var statApplier = Object.FindFirstObjectByType<Castle_Stat_Applier>();
        if (statApplier != null)
        {
            statApplier.ApplyStatsToBuildings(newLevel); // 새 레벨을 전달
        }


        // UI 갱신
        castleLevelTexts[uiIndex].text = $"Level: {newLevel}";
        castlePriceTexts[uiIndex].text = $"HP: {newHealth} / Cost: {upgradeCost}";
        castleUpgradeCostTexts[uiIndex].text = newLevel >= 5 ? "Max Level" : $"Upgrade Cost: {gameStatsData.GetDefaultBuildingStats(buildingName, newLevel + 1).upgradeCost}";

        Debug.Log($"✅ {buildingName} 업그레이드 완료 → Lv.{newLevel}, HP: {newHealth}, 비용: {upgradeCost}");
    }


    //건물 초기화
    void InitializeBuildings()
    {
        int uiIndex = 0; // 👈 UI 배열 인덱스

        for (int i = 0; i < gameStatsData.shopBuildingStatsList.Count; i++)
        {
            var building = gameStatsData.shopBuildingStatsList[i];

            if (!buildingLevels.ContainsKey(building.buildingName))
                buildingLevels[building.buildingName] = 1;

            int currentLevel = buildingLevels[building.buildingName];
            var (health, _, buyCost) = gameStatsData.GetShopBuildingStats(building.buildingName, currentLevel);


            string upgradeCostText = "Max Level";
            if (currentLevel < 5)
            {
                var (_, UpgradeCost, _) = gameStatsData.GetShopBuildingStats(building.buildingName, currentLevel + 1);
                upgradeCostText = $"Upgrade Cost : {UpgradeCost}";
            }

            int buildingIndex = i; // 게임 데이터에서 빌딩 index
            buyBuildingButtons[uiIndex].onClick.AddListener(() => BuyBuilding(buildingIndex));
            upgradeBuildingButtons[uiIndex].onClick.AddListener(() => UpgradeBuilding(buildingIndex));

            // 📌 UI 업데이트
            buildingNameTexts[uiIndex].text = building.buildingName;
            buildingLevelTexts[uiIndex].text = $"Level: {currentLevel}";
            buildingCostTexts[uiIndex].text = $"HP : {health} / Cost : {buyCost}";
            buildingUpgradeCostTexts[uiIndex].text = upgradeCostText;

            uiIndex++; // UI 인덱스는 MainBuilding을 제외하고 증가
        }
    }


       //건물 구매
    void BuyBuilding(int index)
    {
        var building = gameStatsData.shopBuildingStatsList[index];
        int currentLevel = buildingLevels[building.buildingName];
        var (health,_, buildingCost) = gameStatsData.GetShopBuildingStats(building.buildingName, currentLevel);

        if (playerStats.gold < buildingCost)
        {
            Debug.Log("❌ 골드가 부족합니다! 건물을 구매할 수 없습니다.");
            return;
        }

        // 빌딩 C, D는 전용 MAP UI 열기
        if (building.buildingName == "Building C" || building.buildingName == "Building D")
        {
            pendingBuildingName = building.buildingName;
            pendingBuildingLevel = currentLevel;
            pendingBuildingHealth = health;

            if (building.buildingName == "Building C")
            {
                MAP_BuildingC.SetActive(true);
            }
            else if (building.buildingName == "Building D")
            {
                MAP_BuildingD.SetActive(true);
            }

            return;
        }

        // 기본 배치 처리 (C/D 외 일반 건물)
        bool placementSuccess = placementManager.StartBuildingPlacement(building.buildingName, currentLevel, health);

        if (placementSuccess)
        {
            playerStats.gold -= buildingCost;
            shopPanel.SetActive(false);
            IsShopOpen = false;
        }
        else
        {
            Debug.Log("❌ 설치에 실패했습니다. 골드 차감하지 않음.");
        }
    }

    public void OnSlotSelected(string parentName, string slotName)
    {
        GameObject parent = GameObject.Find(parentName);
        if (parent == null)
        {
            Debug.Log($"❌ 배치 실패 -  부모 오브젝트 '{parentName}'을 찾을 수 없습니다.");
            return;
        }

        Transform slotTransform = parent.transform.Find(slotName);
        if (slotTransform == null)
        {
            Debug.Log($"❌ 슬롯 '{slotName}'을 '{parentName}' 안에서 찾을 수 없습니다.");
            return;
        }

        if (slotTransform.childCount > 0)
        {
            Debug.Log($"⚠️ 슬롯 '{slotName}'은 이미 사용 중입니다.");
            return;
        }

        bool placed = placementManager.PlaceBuildingInSlot(
            pendingBuildingName,
            pendingBuildingLevel,
            pendingBuildingHealth,
            slotTransform
        );

        if (placed)
        {
            playerStats.gold -= gameStatsData.GetShopBuildingStats(pendingBuildingName, pendingBuildingLevel).Item2;

            if (pendingBuildingName == "Building C") MAP_BuildingC.SetActive(false);
            else if (pendingBuildingName == "Building D") MAP_BuildingD.SetActive(false);

            Debug.Log($"✅ {pendingBuildingName}가 {parentName}/{slotName}에 배치됨");
        }
        else
        {
            Debug.LogError("❌ 배치 실패");
        }
    }




    // 건물 업그레이드
    void UpgradeBuilding(int index)
    {
        if (index < 0 || index >= gameStatsData.shopBuildingStatsList.Count) // ✅ 변경
        {
            Debug.LogWarning("⚠️ 유효하지 않은 빌딩 인덱스입니다!");
            return;
        }

        var building = gameStatsData.shopBuildingStatsList[index]; // ✅ 변경

        int currentLevel = buildingLevels[building.buildingName];

        if (currentLevel >= 5)
        {
            Debug.Log("⚠️ 최대 레벨 도달!");
            return;
        }

        var (newHealth,UpgrageCost, BuyCost) = gameStatsData.GetShopBuildingStats(building.buildingName, currentLevel + 1); // ✅ 변경

        if (playerStats.gold >= UpgrageCost)
        {
            playerStats.gold -= UpgrageCost;
            buildingLevels[building.buildingName]++;
            int newLevel = buildingLevels[building.buildingName];

            string upgradeCostText = "Max Level";
            if (newLevel < 5)
            {
                var (_, nextUpgradeCost,_) = gameStatsData.GetShopBuildingStats(building.buildingName, newLevel + 1); // ✅ 변경
                upgradeCostText = $"Upgrade Cost : {nextUpgradeCost}";
            }

            buildingLevelTexts[index].text = $"Level: {newLevel}"; // ✅ index-1 → index (UI 인덱스 = shop index로 맞춰짐)
            buildingCostTexts[index].text = $"HP : {newHealth} / Cost : {BuyCost}";
            buildingUpgradeCostTexts[index].text = upgradeCostText;

            Debug.Log($"✅ [{building.buildingName}] 업그레이드 완료 → 레벨: {newLevel}, HP: {newHealth}, 비용: {BuyCost}");
        }
        else
        {
            Debug.Log("❌ 골드 부족!");
        }
    }




    public void BuyNpc(string npcName)
    {
        // 1. 레벨 확인 (없으면 기본 1)
        if (!npcLevels.ContainsKey(npcName))
            npcLevels[npcName] = 1;

        int currentLevel = npcLevels[npcName];

        // 2. 스탯 가져오기
        var (health, baseAttack, upgradecost) = gameStatsData.GetAllyLevelStats(currentLevel);

        // 3. 직업별 보너스 스탯 가져오기 (Warrior, Archer 등)
        GameStatsData.NPCClass npcClass = (GameStatsData.NPCClass)System.Enum.Parse(typeof(GameStatsData.NPCClass), npcName);
        var (bonusAttack, range, cooldown) = gameStatsData.GetNPCClassStats(npcClass);

        int totalAttack = baseAttack + bonusAttack;
        int cost = upgradecost;

        if (playerStats.gold < cost)
        {
            Debug.Log("❌ 골드 부족으로 NPC를 소환할 수 없습니다.");
            return;
        }

        // 4. 배치 시도
        bool success = npcPlacementManager.StartNpcPlacement(npcName, currentLevel); // 필요하면 스탯도 넘기도록 확장 가능
        if (success)
        {
            playerStats.gold -= cost;
            shopPanel.SetActive(false);
            IsShopOpen = false;

            Debug.Log($"✅ NPC 소환 완료: {npcName} (Lv.{currentLevel}) / ATK: {totalAttack} / HP: {health}");
        }
        else
        {
            Debug.Log("❌ NPC 배치 실패 - 골드 차감 안 됨");
        }
    }

    void InitializeNpcs()
    {
        int uiIndex = 0;

        foreach (GameStatsData.NPCClass npcClass in System.Enum.GetValues(typeof(GameStatsData.NPCClass)))
        {
            string npcName = npcClass.ToString();

            if (!npcLevels.ContainsKey(npcName))
                npcLevels[npcName] = 1;

            int currentLevel = npcLevels[npcName];
            var (health, baseAttack, Cost) = gameStatsData.GetAllyLevelStats(currentLevel);
            var (bonusAttack, range, cooldown) = gameStatsData.GetNPCClassStats(npcClass);

            int totalAttack = baseAttack + bonusAttack;
            int cost = Cost;

            string upgradeCostText = currentLevel >= 5 ? "Max Level" : "Upgrade Cost: 50";
            
            npcLevelTexts[uiIndex].text = $"Level: {currentLevel}";
            npcCostTexts[uiIndex].text = $"Cost: {cost}";
            npcStatTexts[uiIndex].text = $"ATK: {totalAttack} / HP: {health}";
            npcUpgradeCostTexts[uiIndex].text = upgradeCostText;

            int capturedIndex = uiIndex; // 람다 캡처용 인덱스 복사

            buyNpcButtons[uiIndex].onClick.RemoveAllListeners();
            buyNpcButtons[uiIndex].onClick.AddListener(() => BuyNpc(npcName));

            upgradeNpcButtons[uiIndex].onClick.RemoveAllListeners();
            upgradeNpcButtons[uiIndex].onClick.AddListener(() => UpgradeNpc(npcName, capturedIndex));

            uiIndex++;
        }
    }

    void UpgradeNpc(string npcName, int uiIndex)
    {
        if (!npcLevels.ContainsKey(npcName))
            npcLevels[npcName] = 1;

        int currentLevel = npcLevels[npcName];

        if (currentLevel >= 5)
        {
            npcUpgradeCostTexts[uiIndex].text = "Upgrade Cost: X";
            Debug.Log($"⚠️ [{npcName}]는 이미 최대 레벨입니다!");
            return;
        }

        int nextLevel = currentLevel + 1;

        // ✅ 다음 레벨 스탯 불러오기
        var (health, baseAttack, buyCost) = gameStatsData.GetAllyLevelStats(nextLevel);
        var (bonusAttack, _, _) = gameStatsData.GetNPCClassStats((GameStatsData.NPCClass)System.Enum.Parse(typeof(GameStatsData.NPCClass), npcName));
        int totalAttack = baseAttack + bonusAttack;

        // ✅ 업그레이드 비용은 항상 50원 고정
        int upgradeCost = 50;

        if (playerStats.gold < upgradeCost)
        {
            Debug.Log("❌ 골드 부족으로 업그레이드 불가");
            return;
        }

        // 💰 골드 차감 + 레벨 증가
        playerStats.gold -= upgradeCost;
        npcLevels[npcName] = nextLevel;

        // 📌 UI 갱신
        npcLevelTexts[uiIndex].text = $"Level: {nextLevel}";
        npcCostTexts[uiIndex].text = $"Cost: {buyCost}"; // ✅ 구매 비용은 buyCost
        npcStatTexts[uiIndex].text = $"ATK: {totalAttack} / HP: {health}";
        npcUpgradeCostTexts[uiIndex].text = (nextLevel >= 5) ? "Upgrade Cost: X" : "Upgrade Cost: 50";

        Debug.Log(
            $"✅ [{npcName}] 업그레이드 성공!\n" +
            $"→ 레벨: {currentLevel} ➝ {nextLevel}\n" +
            $"→ HP: {health}, ATK: {totalAttack}\n" +
            $"💰 차감된 골드: {upgradeCost}, 잔여 골드: {playerStats.gold}"
        );
    }


}
