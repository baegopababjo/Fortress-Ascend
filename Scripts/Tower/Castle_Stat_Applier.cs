using UnityEngine;

public class Castle_Stat_Applier : MonoBehaviour
{
    public GameStatsData gameStatsData;
    public int level = 1; // 적용할 레벨 (기본 1)

    void Start()
    {
        if (gameStatsData == null)
        {
            Debug.LogError("❌ GameStatsData가 연결되지 않았습니다!");
            return;
        }

        gameStatsData.Initialize(); // ✅ 꼭 초기화
        ApplyStatsToBuildings(level);
    }

    public void ApplyStatsToBuildings(int levels)
    {
        var buildings = GetComponentsInChildren<BuildingDamageHandler>();

        foreach (var building in buildings)
        {
            string name = building.gameObject.name;
            string targetStatName = null;

            // ✅ 이름 기반으로 기본 설치 빌딩 판단
            if (name.Contains("Main"))
                targetStatName = "MainBuilding";
            else if (name.Contains("Wall"))
                targetStatName = "Building 2";
            else if (name.Contains("Tower"))
                targetStatName = "Building 1";

            if (targetStatName != null)
            {
                // ✅ 기본 설치 빌딩의 스탯 적용
                var (health, upgradeCost) = gameStatsData.GetDefaultBuildingStats(targetStatName, levels);

                if (health > 0)
                {
                    building.InitializeBuilding(targetStatName, levels, health);
                    // Debug.Log($"✅ {name} → {targetStatName} 레벨 {level} 스탯 적용됨 | 체력: {health}, 비용: {upgradeCost}");
                }
                else
                {
                    Debug.LogWarning($"⚠️ {targetStatName} 스탯을 찾을 수 없습니다! 이름 또는 레벨 확인 필요");
                }
            }
            else
            {
                Debug.Log($"ℹ️ {name}은 Main/Tower/Wall이 아니라 스탯 적용 안 됨");
            }
        }
    }
}
