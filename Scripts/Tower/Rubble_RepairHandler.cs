using UnityEngine;

public class Rubble_RepairHandler : MonoBehaviour
{
    public GameObject RepairPrefab;
    public Vector3 pos;
    public Quaternion rot;

    [Header("💬 UI 연결 (인스펙터에서 설정)")]
    public GameObject repUI;

    [Header("📊 재건 시 필요한 정보")]
    public GameStatsData gameStatsData;
    public int buildingLevel = 1;
    private string buildingName; // 👉 어떤 건물의 잔해인지 저장

    void Start()
    {
        if (repUI != null)
        {
            repUI.SetActive(false);
            //Debug.Log("🧱 Rubble_RepairHandler 초기화 완료, UI 비활성화됨");
        }
        else
        {
            Debug.LogWarning("⚠️ repUI가 인스펙터에 설정되지 않았습니다!");
        }
    }

    // ✅ 잔해 생성 시 원래 건물 정보 세팅
    public void InitializeRubble(string buildingName, int level, Vector3 position, Quaternion rotation)
    {
        this.buildingName = buildingName;
        this.buildingLevel = level;
        this.pos = position;
        this.rot = rotation;

        RepairPrefab.name = buildingName; // 재건용 프리팹 이름도 명확하게 설정
    }

    public void Repair()
    {
        Debug.Log("🔧 Repair() 호출됨 - 건물 재건 시작");

        if (gameStatsData == null)
        {
            gameStatsData = Resources.Load<GameStatsData>("GameStatsData");

            if (gameStatsData == null)
            {
                Debug.LogError("❌ GameStatsData를 Resources에서 찾을 수 없습니다. Repair 중단됨.");
                return;
            }
            else
            {
                //Debug.Log("✅ GameStatsData를 Resources에서 자동으로 불러왔습니다.");
            }
        }

        gameStatsData.Initialize();


        GameObject newBuilding = Instantiate(RepairPrefab, pos, rot);
        //Debug.Log("✅ RepairPrefab 인스턴스 생성 완료");
        BuildingDamageHandler damageHandler = newBuilding.GetComponent<BuildingDamageHandler>();

        if (damageHandler != null)
        {
            string targetStatName = buildingName;

            // ✅ buildingName으로 정확히 판단
            //Debug.Log($"{buildingName} 재건시도");
            //if (buildingName.Contains("Main")) targetStatName = "MainBuilding";
            //else if (buildingName.Contains("Wall")) targetStatName = "Building 2";
            //else if (buildingName.Contains("Tower")) targetStatName = "Building 1";

            if (targetStatName != null)
            {
                var (health, _) = gameStatsData.GetDefaultBuildingStats(targetStatName, buildingLevel);
                damageHandler.InitializeBuilding(targetStatName, buildingLevel, health);
                Debug.Log($"🏗️ {targetStatName} 재건 완료 | 레벨: {buildingLevel}, 체력: {health}");
            }
            else
            {
                Debug.LogWarning($"⚠️ {buildingName} 으로부터 스탯 이름을 판단할 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ BuildingDamageHandler 연결 실패");
        }

        Destroy(gameObject); // 잔해 제거
        //Debug.Log("❌ 잔해물 제거됨");
    }

    public void ShowUI()
    {
        if (repUI != null)
        {
            repUI.SetActive(true);
            //Debug.Log("👁️ 상호작용 UI 표시됨");
        }
        else
        {
            //Debug.LogWarning("⚠️ ShowUI 호출됨 - repUI가 null임");
        }
    }

    public void HideUI()
    {
        if (repUI != null)
        {
            repUI.SetActive(false);
            //Debug.Log("🙈 상호작용 UI 숨김");
        }
        else
        {
            //Debug.LogWarning("⚠️ HideUI 호출됨 - repUI가 null임");
        }
    }
}
