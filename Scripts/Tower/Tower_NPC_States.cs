using UnityEngine;
using static UnityEngine.Rendering.HighDefinition.ScalableSettingLevelParameter;

public class Tower_NPC_Stats : MonoBehaviour
{
    public GameStatsData gameStatsData; // ScriptableObject 연결

    public enum Tower_NPC_Class
    {
        Crossbow, // 🏹 대형 석궁
        Cannon   // 대포
    }

    public int level { get; private set; } = 1;
    public int health { get; private set; }
    public int baseAttackPower { get; private set; }
    public Tower_NPC_Class TnpcClass { get; private set; }
    public int attackPower { get; private set; }
    public float attackRange { get; private set; }
    public float AttackCooldown { get; private set; }

    //public float temprange = 1500.0f;

    private string npcTag; // NPC가 아군인지 적군인지 구별하는 태그

    void Start()
    {
        //level = 1;
    }

    public void SetTowerNPC(Transform tr, string cw, int level)
    {
        if (gameStatsData == null)
        {
            Debug.LogError("❌ Tower_NPC_Stats의 gameStatsData가 null입니다! 초기화 실패.");
            return;
        }
        gameStatsData.Initialize();

        npcTag = tr.tag; // NPC가 아군인지 적군인지 태그 저장
        if (cw.ToLower() == "crossbow") { TnpcClass = Tower_NPC_Class.Crossbow; }
        else if (cw.ToLower() == "cannon") { TnpcClass = Tower_NPC_Class.Cannon; }

        SetLevelStats(level, cw);
        ApplyClassStats();
    }

    public void SetLevelStats(int newLevel, string cw)
    {
        //Enum.TryParse를 사용하면 "cannon" / "Cannon" 등 대소문자 구분 없이 처리 가능
        if (System.Enum.TryParse(cw, true, out Tower_NPC_Class parsed))
        {
            TnpcClass = parsed;
            var (towerHealth, atk) = gameStatsData.GetTNPCLevelStats((GameStatsData.T_NPCclass)TnpcClass, newLevel);
            level = newLevel;

            // ✅ 기본 빌딩 스탯 가져오기
            string buildingName = TnpcClass == Tower_NPC_Class.Cannon ? "Building C" : "Building D";
            var (baseHealth, _,_) = gameStatsData.GetShopBuildingStats(buildingName, level);

            // ✅ 합산 체력 계산
            health = baseHealth + towerHealth;
            baseAttackPower = atk;
        }
    }

    public void ApplyClassStats()
    {
        var (attackBonus, range, cooldown) = gameStatsData.GetTNPCclassStats((GameStatsData.T_NPCclass)TnpcClass);
        attackPower = baseAttackPower + attackBonus;
        attackRange = range;
        AttackCooldown = cooldown;

        Debug.Log($"🆕 {npcTag} {TnpcClass} 설정됨! 💟체력: {health} | 💪공격력: {attackPower} | 🎯 사거리: {attackRange} | ⏳ 쿨타임: {AttackCooldown}");
    }
}
